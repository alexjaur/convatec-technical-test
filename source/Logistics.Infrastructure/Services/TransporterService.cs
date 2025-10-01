using Logistics.Application.Services;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Services
{
    public class TransporterService(LogisticsDbContext dbContext) : ITransporterService
    {

        public Task<List<Transporter>> GetAllAsync(CancellationToken cancellationToken = default) 
            => dbContext.Transporters.AsNoTracking().ToListAsync(cancellationToken);
        
        public Task<Transporter?> GetAsync(int id, CancellationToken cancellationToken) 
            => dbContext.Transporters.FindAsync([id], cancellationToken).AsTask();

        public async Task<Transporter> CreateAsync(Transporter transporter, CancellationToken cancellationToken = default)
        {
            dbContext.Transporters.Add(transporter);

            await dbContext.SaveChangesAsync(cancellationToken);

            return transporter;
        }

        public async Task<Transporter?> UpdateAsync(Transporter transporter, CancellationToken cancellationToken = default)
        {
            var existing = await dbContext.Transporters.FindAsync([transporter.Id], cancellationToken);
            if (existing is null)
            {
                return null;
            }

            dbContext.Entry(existing).CurrentValues.SetValues(transporter);

            await dbContext.SaveChangesAsync(cancellationToken);

            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await dbContext.Transporters.FindAsync([id], cancellationToken: cancellationToken);
            if (existing is null)
            {
                return false;
            }

            dbContext.Transporters.Remove(existing);

            return await dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
