using Core.Authentication.API.Contracts.Data;

namespace Core.Authentication.API.Repositories;

public interface ICustomerRepository
{
    
    Task<bool> CreateAsync(CustomerDto customer, CancellationToken cancellationToken);

    Task<CustomerDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(CustomerDto customer, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}