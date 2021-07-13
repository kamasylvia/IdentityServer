using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public class LotteryRepository : RepositoryBase, IFindByUserIdRepository<Lottery>
    {
        public LotteryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAsync(Lottery item) =>
            await _context.LotteryResults.AddAsync(item);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.LotteryResults.AnyAsync(lr => lr.Id == id);

        public async Task<IEnumerable<Lottery>> FindAllAsync() =>
            await _context.LotteryResults.ToListAsync();

        public async Task<Lottery> FindByIdAsync(params object[] keyValues) =>
            await _context.LotteryResults.FindAsync(keyValues);

        public async Task<IEnumerable<Lottery>> FindByUserIdAsync(Guid userId) =>
            await _context.LotteryResults.Where(a => a.WinnerId == userId).ToListAsync();

        public void Remove(Lottery item) => _context.LotteryResults.Remove(item);

        public void Update(Lottery item) => _context.LotteryResults.Update(item);
    }
}