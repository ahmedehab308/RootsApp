namespace RootsApp.Models.ViewModel
{
    public class CreateInvoiceViewModel
    {
        public int UserId { get; set; }
        public List<InvoiceDetailViewModel> InvoiceDetails { get; set; } = new List<InvoiceDetailViewModel>();

        public string Notes { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class InvoiceDetailViewModel
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }

    public class InvoiceCreateViewModel
    {
        public int UserId { get; set; }

        public List<InvoiceDetail> InvoiceDetails { get; set; } = new();
    }

}
