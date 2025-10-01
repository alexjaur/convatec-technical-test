using Logistics.Domain.Entities;

namespace Logistics.Application.Services
{
    public interface ITransporterService
    {
        Task<List<Transporter>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Transporter?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<Transporter> CreateAsync(Transporter transporter, CancellationToken cancellationToken = default);
        Task<Transporter?> UpdateAsync(Transporter transporter, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
