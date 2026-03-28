using Core.Entities;

namespace Core.Interfaces;

public interface IFaultStatusLogRepository
{
    Task<FaultStatusLog?> GetByIdAsync(int id);
    Task<IEnumerable<FaultStatusLog>> GetAllAsync();
    Task<FaultStatusLog> CreateAsync(FaultStatusLog faultStatusLog);
    Task<FaultStatusLog> UpdateAsync(FaultStatusLog faultStatusLog);
    Task<bool> DeleteAsync(int id);
}