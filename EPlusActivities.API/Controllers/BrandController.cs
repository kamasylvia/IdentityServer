﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.BrandDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EPlusActivities.API.Controllers
{
    [ApiController]
    [EPlusActionFilterAttribute]
    [Route("api/[controller]")]
    public class BrandController : Controller
    {
        private readonly INameExistsRepository<Brand> _brandRepository;
        private readonly IMapper _mapper;

        public BrandController(
            INameExistsRepository<Brand> brandRepository,
            IMapper mapper)
        {
            _brandRepository = brandRepository
                ?? throw new ArgumentNullException(nameof(brandRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<BrandDto>> GetByIdAsync([FromBody] BrandForGetByIdDto brandDto)
        {
            var brand = await _brandRepository.FindByIdAsync(brandDto.Id.Value);
            return brand is null
                ? NotFound($"Could not find the brand.")
                : Ok(_mapper.Map<BrandDto>(brand));
        }

        [HttpGet("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<BrandDto>> GetByNameAsync([FromBody] BrandForGetByNameDto brandDto)
        {
            var brand = await _brandRepository.FindByNameAsync(brandDto.Name);
            return brand is null
                ? NotFound($"Could not find the brand.")
                : Ok(_mapper.Map<BrandDto>(brand));
        }

        [HttpPost]
        [Authorize(Policy = "TestPolicy")]
        public async Task<ActionResult<BrandDto>> CreateAsync([FromBody] BrandForGetByNameDto brandDto)
        {

            #region Parameter validation
            if (await _brandRepository.ExistsAsync(brandDto.Name))
            {
                return Conflict($"The brand is already existed");
            }
            #endregion

            #region New an entity
            var brand = _mapper.Map<Brand>(brandDto);
            // brand.Id = Guid.NewGuid();
            #endregion

            #region Database operations
            await _brandRepository.AddAsync(brand);
            var succeeded = await _brandRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok(_mapper.Map<BrandDto>(brand))
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpPatch("name")]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> UpdateNameAsync([FromBody] BrandDto brandDto)
        {
            #region Parameter validation
            if (!await _brandRepository.ExistsAsync(brandDto.Id.Value))
            {
                return NotFound($"Could not find the brand.");
            }
            #endregion

            #region Database operations
            var brand = await _brandRepository.FindByIdAsync(brandDto.Id.Value);
            brand = _mapper.Map<BrandDto, Brand>(brandDto, brand);
            // var brand = _mapper.Map<Brand>(brandDto);
            _brandRepository.Update(brand);
            var succeeded = await _brandRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }

        [HttpDelete]
        [Authorize(Policy = "TestPolicy")]
        public async Task<IActionResult> DeleteAsync([FromBody] BrandDto brandDto)
        {
            #region Parameter validation
            if (!await _brandRepository.ExistsAsync(brandDto.Id.Value)
                || !await _brandRepository.ExistsAsync(brandDto.Name))
            {
                return NotFound($"Could not find the brand '{brandDto.Name}'");
            }
            #endregion

            #region Database operations
            var brand = await _brandRepository.FindByIdAsync(brandDto.Id.Value);
            brand = _mapper.Map<BrandDto, Brand>(brandDto, brand);
            _brandRepository.Remove(brand);
            var succeeded = await _brandRepository.SaveAsync();
            #endregion

            return succeeded
                ? Ok()
                : new InternalServerErrorObjectResult("Update database exception");
        }
    }
}
