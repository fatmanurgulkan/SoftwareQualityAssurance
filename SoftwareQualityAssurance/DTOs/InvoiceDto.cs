namespace SoftwareQualityAssurance.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class CreateInvoiceDto
{
    public string SerialNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateInvoiceDto
{
    public string SerialNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
}

