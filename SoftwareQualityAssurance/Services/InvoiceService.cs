using Microsoft.EntityFrameworkCore;
using SoftwareQualityAssurance.Data;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;

namespace SoftwareQualityAssurance.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly ApplicationDbContext _context;

    public InvoiceService(
        IRepository<Invoice> invoiceRepository,
        IRepository<Customer> customerRepository,
        ApplicationDbContext context)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _context = context;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Customer)
            .ToListAsync();

        return invoices.Select(MapToDto).Where(dto => dto != null).Cast<InvoiceDto>();
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto)
    {
        // Business rule: Invoice amount must be greater than zero
        if (createDto.TotalAmount <= 0)
        {
            throw new InvalidOperationException("Invoice amount must be greater than zero.");
        }

        // Validate Customer exists
        if (!await _customerRepository.ExistsAsync(createDto.CustomerId))
        {
            throw new InvalidOperationException($"Customer with ID {createDto.CustomerId} does not exist.");
        }

        var invoice = new Invoice
        {
            SerialNumber = createDto.SerialNumber,
            TotalAmount = createDto.TotalAmount,
            InvoiceDate = createDto.InvoiceDate,
            CustomerId = createDto.CustomerId,
            Status = createDto.Status
        };

        var createdInvoice = await _invoiceRepository.AddAsync(invoice);

        // Reload with includes
        var invoiceWithIncludes = await _context.Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == createdInvoice.Id);

        if (invoiceWithIncludes == null)
        {
            throw new InvalidOperationException("Failed to retrieve created invoice.");
        }

        return MapToDto(invoiceWithIncludes);
    }

    public async Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
            return null;

        // Business rule: Invoice amount must be greater than zero
        if (updateDto.TotalAmount <= 0)
        {
            throw new InvalidOperationException("Invoice amount must be greater than zero.");
        }

        // Validate Customer exists
        if (!await _customerRepository.ExistsAsync(updateDto.CustomerId))
        {
            throw new InvalidOperationException($"Customer with ID {updateDto.CustomerId} does not exist.");
        }

        invoice.SerialNumber = updateDto.SerialNumber;
        invoice.TotalAmount = updateDto.TotalAmount;
        invoice.InvoiceDate = updateDto.InvoiceDate;
        invoice.CustomerId = updateDto.CustomerId;
        invoice.Status = updateDto.Status;

        await _invoiceRepository.UpdateAsync(invoice);

        // Reload with includes
        var invoiceWithIncludes = await _context.Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id);

        return MapToDto(invoiceWithIncludes!);
    }

    public async Task<bool> DeleteInvoiceAsync(int id)
    {
        return await _invoiceRepository.DeleteAsync(id);
    }

    private static InvoiceDto? MapToDto(Invoice invoice)
    {
        if (invoice == null)
            return null;

        return new InvoiceDto
        {
            Id = invoice.Id,
            SerialNumber = invoice.SerialNumber,
            TotalAmount = invoice.TotalAmount,
            InvoiceDate = invoice.InvoiceDate,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer != null 
                ? $"{invoice.Customer.FirstName} {invoice.Customer.LastName}".Trim() 
                : string.Empty,
            Status = invoice.Status,
            CreatedDate = invoice.CreatedDate
        };
    }
}

