﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.LotteryDtos;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    /// <summary>
    /// 抽奖记录 API
    /// </summary>
    [Route("choujiang/api/[controller]")]
    [ApiController]
    [EPlusActionFilterAttribute]
    public class LotteryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<LotteryController> _logger;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityRepository _activityRepository;
        private readonly IPrizeItemRepository _prizeItemRepository;
        private readonly IFindByParentIdRepository<ActivityUser> _activityUserRepository;
        private readonly IRepository<Coupon> _couponRepository;
        private readonly IFindByParentIdRepository<PrizeTier> _prizeTypeRepository;
        private readonly ILotteryService _lotteryService;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IGeneralLotteryRecordsRepository _generalLotteryRecordsRepository;
        private readonly IMemberService _memberService;

        public LotteryController(
            ILotteryRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            ILogger<LotteryController> logger,
            IFindByParentIdRepository<ActivityUser> activityUserRepository,
            IRepository<Coupon> couponResponseDto,
            ILotteryService lotteryService,
            IMemberService memberService,
            IIdGeneratorService idGeneratorService,
            IGeneralLotteryRecordsRepository generalLotteryRecordsRepository
        )
        {
            _idGeneratorService =
                idGeneratorService ?? throw new ArgumentNullException(nameof(idGeneratorService));
            _memberService =
                memberService ?? throw new ArgumentNullException(nameof(memberService));
            _generalLotteryRecordsRepository =
                generalLotteryRecordsRepository
                ?? throw new ArgumentNullException(nameof(generalLotteryRecordsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
            _lotteryService =
                lotteryService ?? throw new ArgumentNullException(nameof(lotteryService));
            _activityUserRepository =
                activityUserRepository
                ?? throw new ArgumentNullException(nameof(activityUserRepository));
            _couponRepository =
                couponResponseDto ?? throw new ArgumentNullException(nameof(couponResponseDto));
            _lotteryRepository =
                lotteryRepository ?? throw new ArgumentNullException(nameof(lotteryRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _activityRepository =
                activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _prizeItemRepository =
                prizeItemRepository ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _prizeTypeRepository =
                prizeTypeRepository ?? throw new ArgumentNullException(nameof(prizeTypeRepository));
        }

        /// <summary>
        /// 获取某个用户的抽奖记录
        /// </summary>
        /// <param name="lotteryDto"></param>
        /// <returns></returns>
        [HttpGet("customer/lottery-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetLotteryRecordsByUserIdAsync(
            [FromQuery] LotteryForGetByUserIdDto lotteryDto
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            var result = await FindLotteryRecordsAsync(lotteryDto.UserId.Value);
            return result.Count() > 0
                ? Ok(result.OrderBy(x => x.DateTime))
                : NotFound("Could not find the lottery results.");
        }

        /// <summary>
        /// 获取某个用户的中奖记录
        /// </summary>
        /// <param name="lotteryDto"></param>
        /// <returns></returns>
        [HttpGet("customer/lucky-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> GetWinningRecordsByUserIdAsync(
            [FromQuery] LotteryForGetByUserIdDto lotteryDto
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(lotteryDto.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }
            #endregion

            var records = await FindLotteryRecordsAsync(lotteryDto.UserId.Value);
            var result = records.Where(record => record.IsLucky);
            return result.Count() > 0
                ? Ok(result.OrderBy(x => x.DateTime))
                : NotFound("Could not find the winning results.");
        }

        private async Task<IEnumerable<LotteryDto>> FindLotteryRecordsAsync(Guid userId)
        {
            var lotteries = await _lotteryRepository.FindByUserIdAsync(userId);

            // 因为进行了全剧配置，AutoMapper 在此执行
            // _mapper.Map<IEnumerable<LotteryDto>>(lotteries)
            // 时会自动转换 DateTime 导致精确时间丢失，
            // 所以这里手动添加精确时间。
            var result = lotteries.Select(
                x =>
                {
                    var resultItem = _mapper.Map<LotteryDto>(x);
                    resultItem.DateTime = x.DateTime;
                    resultItem.PickedUpTime = x.PickedUpTime;
                    return resultItem;
                }
            );

            return result;
        }

        /// <summary>
        /// 管理员根据活动号查询中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/detailed-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<
            ActionResult<LotteryRecordsForManagerResponse>
        > GetLotteryRecordsForManagerAsync([FromQuery] LotteryRecordsForManagerRequest request)
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var lotteries = await activity.LotteryResults.Where(
                    lr =>
                        lr.IsLucky
                        && Enum.Parse<ChannelCode>(request.Channel, true) == lr.ChannelCode
                        && !(request.StartTime > lr.DateTime)
                        && !(lr.DateTime > request.EndTime)
                )
                .ToAsyncEnumerable()
                .SelectAwait(async l => await _lotteryRepository.FindByIdAsync(l.Id))
                .ToListAsync();
            #endregion
            return Ok(_lotteryService.CreateLotteryForDownload(lotteries));
        }

        /// <summary>
        /// 管理员根据活动号下载中奖记录报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/excel")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<IActionResult> DownloadLotteryRecordsForManagerAsyncs(
            [FromQuery] LotteryRecordsForManagerRequest request
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var lotteries = await activity.LotteryResults.Where(
                    lr =>
                        lr.IsLucky
                        && Enum.Parse<ChannelCode>(request.Channel, true) == lr.ChannelCode
                        && !(request.StartTime > lr.DateTime)
                        && !(lr.DateTime > request.EndTime)
                )
                .ToAsyncEnumerable()
                .SelectAwait(async l => await _lotteryRepository.FindByIdAsync(l.Id))
                .ToListAsync();
            #endregion

            var generalLotteryRecords = await _generalLotteryRecordsRepository.FindByDateRangeAsync(
                activity.Id.Value,
                Enum.Parse<ChannelCode>(request.Channel, true),
                request.StartTime,
                request.EndTime
            );

            var (memoryString, contentType) = _lotteryService.DownloadLotteryRecords(
                generalLotteryRecords,
                lotteries
            );

            return File(memoryString, contentType);
        }

        /// <summary>
        /// 根据活动号和日期查询抽奖数、中奖数、兑换数
        /// </summary>
        /// <returns></returns>
        [HttpGet("manager/general-records")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "manager, tester"
        )]
        public async Task<
            ActionResult<IEnumerable<LotteryForGetGeneralRecordsResponse>>
        > GetGeneralStatementsAsync([FromQuery] LotteryForGetGeneralRecordsRequest request)
        {
            #region Parameter validation
            var channel = Enum.Parse<ChannelCode>(request.Channel, true);
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }
            var generalLotteryRecords = await _generalLotteryRecordsRepository.FindByDateRangeAsync(
                activity.Id.Value,
                channel,
                request.StartTime,
                request.EndTime
            );
            #endregion
            return Ok(_mapper.Map<IEnumerable<LotteryForGetGeneralRecordsResponse>>(generalLotteryRecords));
        }

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "customer, tester"
        )]
        public async Task<ActionResult<IEnumerable<LotteryDto>>> CreateAsync(
            [FromBody] LotteryForCreateDto request
        )
        {
            #region Parameter validation
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return NotFound("Could not find the user.");
            }

            var activity = await _activityRepository.FindByIdAsync(request.ActivityId.Value);
            if (activity is null)
            {
                return NotFound("Could not find the activity.");
            }

            if (DateTime.Today < activity.StartTime || DateTime.Today > activity.EndTime)
            {
                return BadRequest("This activity is expired.");
            }

            var activityUser = await _activityUserRepository.FindByIdAsync(
                request.ActivityId.Value,
                request.UserId.Value
            );

            if (activityUser is null)
            {
                return BadRequest("The user had to join the activity first.");
            }

            // 剩余抽奖次数不足
            if (activityUser.RemainingDraws < request.Count)
            {
                return BadRequest("The user did not have enough chance to draw a lottery .");
            }

            // 超过全活动周期抽奖次数限制
            if (activityUser.UsedDraws + request.Count > activity.Limit)
            {
                return BadRequest(
                    "Sorry, the user had already achieved the maximum number of draws of this activity."
                );
            }

            // 今天没登陆过的用户，每日已用抽奖次数清零
            if (!(user.LastDrawDate >= DateTime.Today))
            {
                activityUser.TodayUsedDraws = 0;
                user.LastDrawDate = DateTime.Today;
            }

            // 超过每日抽奖次数限制
            if (activityUser.TodayUsedDraws + request.Count > activity.DailyLimit)
            {
                return BadRequest(
                    "Sorry, the user had already achieved the daily maximum number of draws of this activity."
                );
            }
            var statement = await _generalLotteryRecordsRepository.FindByDateAsync(
                request.ActivityId.Value,
                Enum.Parse<ChannelCode>(request.ChannelCode, true),
                DateTime.Today
            );
            var requireNewStatement = statement is null;
            if (requireNewStatement)
            {
                statement = new GeneralLotteryRecords
                {
                    Activity = activity,
                    DateTime = DateTime.Today
                };
            }
            #endregion

            #region Consume the draws
            activityUser.RemainingDraws -= request.Count;
            activityUser.TodayUsedDraws += request.Count;
            activityUser.UsedDraws += request.Count;
            statement.Draws += request.Count;
            #endregion

            #region Generate the lottery result
            var response = new List<LotteryDto>();

            for (int i = 0; i < request.Count; i++)
            {
                var lottery = _mapper.Map<Lottery>(request);
                lottery.User = user;
                lottery.Activity = activity;
                lottery.DateTime = DateTime.Now;

                (lottery.PrizeTier, lottery.PrizeItem) = await _lotteryService.DrawPrizeAsync(
                    activity
                );

                if (lottery.PrizeTier is not null)
                {
                    lottery.IsLucky = true;
                    statement.Winners++;

                    switch (lottery.PrizeItem.PrizeType)
                    {
                        case PrizeType.Credit:
                            var (updateCreditResult, updateCreditResponseDto) =
                                await _memberService.UpdateCreditAsync(
                                    user.Id,
                                    new MemberForUpdateCreditRequestDto
                                    {
                                        memberId = user.MemberId,
                                        points = lottery.PrizeItem.Credit.Value,
                                        reason = "积分奖品",
                                        sheetId = _idGeneratorService.NextId().ToString(),
                                        updateType = CreditUpdateType.Addition
                                    }
                                );
                            if (updateCreditResult)
                            {
                                user.Credit += lottery.PrizeItem.Credit.Value;
                                if (user.Credit != updateCreditResponseDto.Body.Content.NewPoints)
                                {
                                    _logger.LogError(
                                        "Local credits did not equal to the member's new points."
                                    );
                                    return new InternalServerErrorObjectResult(
                                        "Local credits did not equal to the member's new points."
                                    );
                                }
                            }
                            break;
                        case PrizeType.Coupon:
                            var (releaseCouponResult, couponResponseDto) =
                                await _memberService.ReleaseCouponAsync(
                                    new MemberForReleaseCouponRequestDto
                                    {
                                        couponActiveCode = lottery.PrizeItem.CouponActiveCode,
                                        memberId = user.MemberId,
                                        qty = 1,
                                        reason = "优惠券奖品"
                                    }
                                );
                            var coupons = couponResponseDto?.Body?.Content?.HideCouponCode?.Split(
                                    ',',
                                    StringSplitOptions.TrimEntries
                                )
                                .Select(
                                    code =>
                                        new Coupon
                                        {
                                            User = user,
                                            PrizeItem = lottery.PrizeItem,
                                            Code = code
                                        }
                                );

                            var temp = user.Coupons is null
                                ? new List<Coupon>()
                                : user.Coupons.ToList();
                            temp.AddRange(coupons);
                            user.Coupons = temp;

                            temp = lottery.PrizeItem.Coupons is null
                                ? new List<Coupon>()
                                : lottery.PrizeItem.Coupons.ToList();
                            temp.AddRange(coupons);
                            lottery.PrizeItem.Coupons = temp;

                            await coupons.ToAsyncEnumerable()
                                .ForEachAwaitAsync(
                                    async item => await _couponRepository.AddAsync(item)
                                );
                            break;
                        default:
                            break;
                    }
                }

                await _lotteryRepository.AddAsync(lottery);
                var result = _mapper.Map<LotteryDto>(lottery);
                result.DateTime = lottery.DateTime; // Skip auto mapper.
                response.Add(result);
            }
            #endregion

            #region Database operations
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (requireNewStatement)
                await _generalLotteryRecordsRepository.AddAsync(statement);
            else
                _generalLotteryRecordsRepository.Update(statement);

            var succeeded = await _lotteryRepository.SaveAsync();
            if (!succeeded)
            {
                _logger.LogError("Failed to create the lottery");
                return new InternalServerErrorObjectResult("Update database exception");
            }

            if (!userUpdateResult.Succeeded)
            {
                _logger.LogError(userUpdateResult.ToString());
                return new InternalServerErrorObjectResult(userUpdateResult.ToString());
            }
            #endregion

            return Ok(response.OrderBy(x => x.DateTime));
        }

        /// <summary>
        /// 更新抽奖记录
        /// </summary>
        /// <param name="lotteryDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = "AllRoles"
        )]
        public async Task<IActionResult> UpdateAsync([FromBody] LotteryForUpdateDto lotteryDto)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                return NotFound("Could not find the lottery.");
            }
            #endregion

            #region Database operations
            lottery = _mapper.Map<LotteryForUpdateDto, Lottery>(lotteryDto, lottery);
            lottery.PickedUpTime = lotteryDto.PickedUpTime; // Skip auto mapper.
            _lotteryRepository.Update(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            if (succeeded)
            {
                return Ok();
            }

            _logger.LogError("Failed to update the lottery");
            return new InternalServerErrorObjectResult("Update database exception");
        }

        /// <summary>
        /// 删除抽奖记录
        /// </summary>
        /// <param name="lotteryDto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "admin, tester"
        )]
        public async Task<IActionResult> DeleteAsync([FromBody] LotteryForGetByIdDto lotteryDto)
        {
            var lottery = await _lotteryRepository.FindByIdAsync(lotteryDto.Id.Value);

            #region Parameter validation
            if (lottery is null)
            {
                return NotFound("Could not find the the lottery.");
            }
            #endregion

            #region Database operations
            _lotteryRepository.Remove(lottery);
            var succeeded = await _lotteryRepository.SaveAsync();
            #endregion

            if (succeeded)
            {
                return Ok();
            }
            _logger.LogError("Failed to delete the lottery");
            return new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
