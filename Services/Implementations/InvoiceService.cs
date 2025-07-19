using RootsApp.Models;
using RootsApp.Repositories.Implementations;
using RootsApp.Repositories.Interfaces;
using RootsApp.Services.Interfaces;

namespace RootsApp.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;

        public InvoiceService(IInvoiceRepository invoiceRepo)
        {
            _invoiceRepo = invoiceRepo;
        }

        public IEnumerable<Invoice> GetAll()
        {
            return _invoiceRepo.GetAll();
        }

        public List<Invoice> GetAllByUserId(int userId)
        {
            return _invoiceRepo.GetAllByUserId(userId);
        }

        public Invoice GetById(int id)
        {
            return _invoiceRepo.GetById(id);
        }

        public bool CreateInvoice(Invoice invoice)
        {
            try
            {
                // Prepare data
                invoice.CreatedAt = DateTime.Now;
                invoice.TotalAmount = invoice.InvoiceDetails.Sum(d => d.Quantity * d.Price);

                // Ensure there are details
                if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
                {
                    throw new Exception("Cannot create an invoice without details");
                }

                // Save the invoice
                _invoiceRepo.Add(invoice);
                _invoiceRepo.Save();

                return true;
            }
            catch (Exception ex)
            {
                // Logging can be added here
                Console.WriteLine($"Error while creating the invoice: {ex.Message}");
                throw;
            }
        }

        public void UpdateInvoice(Invoice invoice)
        {
            _invoiceRepo.UpdateInvoice(invoice);
        }

        public void DeleteInvoice(int id)
        {
            _invoiceRepo.DeleteInvoice(id);
        }
    }
}
