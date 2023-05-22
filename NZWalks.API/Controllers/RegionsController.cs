using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDBContext dbContext;
        private readonly IRegionRepository regionRepository;

        private readonly IMapper Mapper;

        public RegionsController(NZWalksDBContext dbContext,IRegionRepository regionRepository,IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            Mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles ="Reader")]
        public async  Task<IActionResult> GetAll()
        {
            var regionsDomain =await  regionRepository.GetAllAsync();
            //var regionsDTO = new List<RegionsDTO>();
            //foreach(var regionDomain in regionsDomain)
            //{
            //    regionsDTO.Add(new RegionsDTO()
            //    {
            //        Id = regionDomain.Id,
            //        Code = regionDomain.Code,
            //        Name = regionDomain.Name,
            //        RegionImageUrl = regionDomain.RegionImageUrl,
            //    });
            //}

           
            return Ok(Mapper.Map<List<RegionsDTO>>(regionsDomain));
             
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain == null)
            {
                return NotFound();
            }
             

            

            return Ok(Mapper.Map<RegionsDTO>(regionDomain));

        }

        [HttpPost]
        [validateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody]AddRegionDTO addRegion)
        {
             
                var regionDomainModel = Mapper.Map<Region>(addRegion);
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                var regionDTO = Mapper.Map<RegionsDTO>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);
                // return Ok(regionDTO);
             

        }

        [HttpPut]
        [Route("{id:Guid}")]
        [validateModel]
        [Authorize(Roles = "Writer")]
        public  async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDTO updateRegionDTO) {

           
                var regionDomainModel = Mapper.Map<Region>(updateRegionDTO);
                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
                if (regionDomainModel == null)
                {
                    return NotFound();
                }



                var regionDto = Mapper.Map<RegionsDTO>(regionDomainModel);
                return Ok(regionDto);
             

        }
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
         var regionDomainModel= await regionRepository.DeleteAsync(id);
            if(regionDomainModel==null)
            {
                return NotFound();
            }
            
             
            return Ok(Mapper.Map<RegionsDTO>(regionDomainModel));
        }

       
    }
}
