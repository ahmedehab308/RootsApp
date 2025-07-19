using System;
using System.Collections.Generic;

namespace RootsApp.Models;

public partial class InvoiceDetail
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public string Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
