﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.PrizeItemDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class PrizeItemController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFindByNameRepository<PrizeItem> _prizeItemRepository;
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly IMapper _mapper;
        private readonly INameExistsRepository<Category> _categoryRepository;

        public PrizeItemController(
            UserManager<ApplicationUser> userManager,
            IFindByNameRepository<PrizeItem> prizeItemRepository,
            INameExistsRepository<Brand> brandRepository,
            INameExistsRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));
            _prizeItemRepository = prizeItemRepository
                ?? throw new ArgumentNullException(nameof(prizeItemRepository));
            _brandRepository = brandRepository
                ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository
                ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<IEnumerable<PrizeItemDto>>> GetByNameAsync([FromBody] PrizeItemForGetByNameDto prizeItemDto) =>
            Ok(_mapper.Map<IEnumerable<PrizeItemDto>>(
                await _prizeItemRepository.FindByNameAsync(prizeItemDto.Name)));

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeItemDto>> GetGetByIdAsync([FromBody] PrizeItemForGetByIdDto prizeItemDto)
        {
            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id.Value);

            return prizeItem is null ? NotFound("Could not find the prizeItem.") : Ok(_mapper.Map<PrizeItemDto>(prizeItem));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<PrizeItemDto>> CreateAsync([FromBody] PrizeItemForCreateDto prizeItemDto)
        {
            #region New an entity
            var prizeItem = _mapper.Map<PrizeItem>(prizeItemDto);
            prizeItem.Brand = await GetBrandAsync(prizeItemDto.BrandName);
            prizeItem.Category = await GetCategoryAsync(prizeItemDto.CategoryName);
            #endregion

            #region Database operations
            await _prizeItemRepository.AddAsync(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<PrizeItemDto>(prizeItem))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPut]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateAsync([FromBody] PrizeItemForUpdateDto prizeItemDto)
        {
            #region Parameter validation
            if (!await _prizeItemRepository.ExistsAsync(prizeItemDto.Id.Value))
            {
                return BadRequest("The prize item is not existed");
            };
            #endregion

            #region New an entity
            var prizeItem = _mapper.Map<PrizeItemForUpdateDto, PrizeItem>(
                prizeItemDto,
                await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id.Value));
            prizeItem.Brand = await GetBrandAsync(prizeItemDto.BrandName);
            prizeItem.Category = await GetCategoryAsync(prizeItemDto.CategoryName);
            #endregion

            #region Database operations
            _prizeItemRepository.Update(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] PrizeItemForGetByIdDto prizeItemDto)
        {
            #region Parameter validation
            if (!await _prizeItemRepository.ExistsAsync(prizeItemDto.Id.Value))
            {
                return BadRequest("The prize item is not existed");
            };
            #endregion

            #region Database operations
            var prizeItem = await _prizeItemRepository.FindByIdAsync(prizeItemDto.Id.Value);
            _prizeItemRepository.Remove(prizeItem);
            var succeeded = await _prizeItemRepository.SaveAsync();

            #endregion
            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        private async Task<Brand> GetBrandAsync(string brandName)
        {
            var brand = await _brandRepository.FindByNameAsync(brandName);
            if (brand is null)
            {
                brand = new Brand(brandName);
                await _brandRepository.AddAsync(brand);
            }
            return brand;
        }

        private async Task<Category> GetCategoryAsync(string categoryName)
        {
            var category = await _categoryRepository.FindByNameAsync(categoryName);
            if (category is null)
            {
                category = new Category(categoryName);
                await _categoryRepository.AddAsync(category);
            }
            return category;
        }
    }
}
