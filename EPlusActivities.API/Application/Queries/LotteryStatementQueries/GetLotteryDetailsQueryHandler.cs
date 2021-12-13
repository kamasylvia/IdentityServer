using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.LotteryStatementDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.LotteryService;
using EPlusActivities.API.Services.MemberService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Application.Queries.LotteryStatementQueries
{
    public class GetLotteryDetailsQueryHandler
        : DrawingRequestHandlerBase,
          IRequestHandler<GetLotteryDetailsQuery, IEnumerable<GetLotteryDetailsResponse>>
    {
        public GetLotteryDetailsQueryHandler(
            ILotteryDetailRepository lotteryRepository,
            UserManager<ApplicationUser> userManager,
            IActivityRepository activityRepository,
            IPrizeItemRepository prizeItemRepository,
            IFindByParentIdRepository<PrizeTier> prizeTypeRepository,
            IMapper mapper,
            IActivityUserRepository activityUserRepository,
            IRepository<Coupon> couponResponseDto,
            ILotteryService lotteryService,
            IMemberService memberService,
            IIdGeneratorService idGeneratorService,
            ILotterySummaryRepository lotterySummaryStatementRepository,
            IActivityService activityService
        )
            : base(
                lotteryRepository,
                userManager,
                activityRepository,
                prizeItemRepository,
                prizeTypeRepository,
                mapper,
                activityUserRepository,
                couponResponseDto,
                lotteryService,
                memberService,
                idGeneratorService,
                lotterySummaryStatementRepository,
                activityService
            ) { }

        public async Task<IEnumerable<GetLotteryDetailsResponse>> Handle(
            GetLotteryDetailsQuery request,
            CancellationToken cancellationToken
        )
        {
            #region Parameter validation
            var activity = await _activityRepository.FindByActivityCodeAsync(request.ActivityCode);
            if (activity is null)
            {
                throw new NotFoundException("Could not find the activity.");
            }
            var lotteries = await activity.LotteryDetailStatement
                .Where(
                    lr =>
                        lr.IsLucky
                        && request.Channel == lr.ChannelCode
                        && !(request.StartTime > lr.DateTime)
                        && !(lr.DateTime > request.EndTime)
                )
                .ToAsyncEnumerable()
                .SelectAwait(async l => await _lotteryRepository.FindByIdAsync(l.Id))
                .ToListAsync();
            #endregion

            return _lotteryService.CreateLotteryForDownload(lotteries);
        }
    }
}