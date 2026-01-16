using SoftwareQualityAssurance.Models;

namespace SoftwareQualityAssurance.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}

