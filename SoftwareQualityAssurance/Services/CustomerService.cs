using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;

namespace SoftwareQualityAssurance.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto)
    {
        // Business rule: Email must be unique
        if (await _customerRepository.EmailExistsAsync(createDto.Email))
        {
            throw new InvalidOperationException($"Email '{createDto.Email}' already exists.");
        }

        var customer = new Customer
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            IdentityNumber = createDto.IdentityNumber,
            Balance = createDto.Balance,
            PhoneNumber = createDto.PhoneNumber
        };

        var createdCustomer = await _customerRepository.AddAsync(customer);
        return MapToDto(createdCustomer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return null;

        // Business rule: Email must be unique (excluding current customer)
        if (await _customerRepository.EmailExistsAsync(updateDto.Email, id))
        {
            throw new InvalidOperationException($"Email '{updateDto.Email}' already exists.");
        }

        customer.FirstName = updateDto.FirstName;
        customer.LastName = updateDto.LastName;
        customer.Email = updateDto.Email;
        customer.IdentityNumber = updateDto.IdentityNumber;
        customer.Balance = updateDto.Balance;
        customer.PhoneNumber = updateDto.PhoneNumber;

        var updatedCustomer = await _customerRepository.UpdateAsync(customer);
        return MapToDto(updatedCustomer);
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        return await _customerRepository.DeleteAsync(id);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            IdentityNumber = customer.IdentityNumber,
            Balance = customer.Balance,
            PhoneNumber = customer.PhoneNumber,
            CreatedDate = customer.CreatedDate
        };
    }
}

