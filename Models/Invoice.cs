using System;
using System.Collections.Generic;

namespace RootsApp.Models;

public partial class Invoice
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual User User { get; set; } = null!;
}
