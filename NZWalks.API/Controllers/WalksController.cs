using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        //CREATE WALK 
        //POST : /api/walks
        [HttpPost]
        [validateModel]

        public async Task<IActionResult> Create([FromBody] AddWalkRequestDTO addWalkRequestDTO)
        {
             
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDTO);
                await walkRepository.CreateAsync(walkDomainModel);
                return Ok(mapper.Map<WalkDTO>(walkDomainModel));
             
        }


        //GET WALKS
        //GET : /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=True&pageNumber=1&pageSize=1000
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize=1000)
        {
            var walksDomainModel = await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending ?? true,pageNumber,pageSize);

            return Ok(mapper.Map<List<WalkDTO>>(walksDomainModel));
        }

        //GET WALK by id
        //GET : /api/walks/{id}

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.GetByIdAsync(id);
            if (walkDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDTO>(walkDomainModel));
        }

        //Update WALK by id
        //PUT : /api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [validateModel]
        public async Task<IActionResult> UpdateById([FromRoute] Guid id,UpdateWalkDTO updateWalkDTO)
        {
            
                var walkDomainModel = mapper.Map<Walk>(updateWalkDTO);
                walkDomainModel = await walkRepository.UpdateByIdAsync(id, walkDomainModel);
                if (walkDomainModel == null)
                {
                    return NotFound();

                }
                return Ok(mapper.Map<WalkDTO>(walkDomainModel));
             
        }

        //DELETE Walk by id
        //DELETE : /api/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteById([FromRoute]Guid id)
        {
          var deletedWalkDomainModel=  await walkRepository.DeleteByIdAsync(id);
            if(deletedWalkDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDTO>(deletedWalkDomainModel));
        }
    }
}
