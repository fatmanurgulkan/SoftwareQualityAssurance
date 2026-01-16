using SoftwareQualityAssurance.DTOs;

namespace SoftwareQualityAssurance.Services;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto);
    Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto);
    Task<bool> DeleteInvoiceAsync(int id);
}

