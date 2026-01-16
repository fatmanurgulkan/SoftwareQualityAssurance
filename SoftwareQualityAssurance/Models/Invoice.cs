namespace SoftwareQualityAssurance.Models;

public class Invoice : BaseEntity
{
    public string SerialNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty; // e.g., "Pending", "Paid", "Cancelled"

    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
}

