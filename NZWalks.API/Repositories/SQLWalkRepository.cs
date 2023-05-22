using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDBContext dBContext;

        public SQLWalkRepository(NZWalksDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<Walk>  CreateAsync(Walk walk)
        {
            await dBContext.Walks.AddAsync(walk);
            await dBContext.SaveChangesAsync();
            return walk;
        }

       public async Task<List<Walk>>  GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //return await dBContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
            var walks = dBContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            if(string.IsNullOrWhiteSpace(filterOn)==false && string.IsNullOrWhiteSpace(filterQuery)==false) {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks=walks.Where(x=>x.Name.Contains(filterQuery)); 
                }
            
            }
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks=isAscending? walks.OrderBy(x=>x.Name):walks.OrderByDescending(x=>x.Name);
                }
            }

            var skipResult = (pageNumber - 1) * pageSize;
            return await walks.Skip(skipResult).Take(pageSize).ToListAsync();
        }

       public async Task<Walk?>  GetByIdAsync(Guid id)
        {
          return await dBContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);


        }

      public async  Task<Walk?>  UpdateByIdAsync(Guid id, Walk walk)
        {
            var existingWalk= await dBContext.Walks.FirstOrDefaultAsync(x=> x.Id == id);
            if (existingWalk == null) { 
            return null;}
            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImgUrl = walk.WalkImgUrl;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;
           return  existingWalk;

        }

       public async Task<Walk?>  DeleteByIdAsync(Guid id)
        {
            var existingWalk=await dBContext.Walks.FirstOrDefaultAsync(x=> x.Id == id);
            if (existingWalk == null)
            {
                return null;
            }
            dBContext.Walks.Remove(existingWalk);
            await dBContext.SaveChangesAsync();
            return existingWalk;
        }
    }
}
