using Microsoft.EntityFrameworkCore;
using RootsApp.Models;
using RootsApp.Repositories.Interfaces;

namespace RootsApp.Repositories.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly RootsDbContext _context;

        public InvoiceRepository(RootsDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Invoice> GetAll()
        {
            return _context.Invoices.Include(i => i.InvoiceDetails).ToList();
        }
        public List<Invoice> GetAllByUserId(int userId)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();
        }

        public Invoice GetById(int id)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefault(i => i.Id == id);
        }

        public void Add(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateInvoice(Invoice invoice)
        {
            var existing = _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefault(i => i.Id == invoice.Id);

            if (existing != null)
            {
                existing.TotalAmount = invoice.InvoiceDetails.Sum(d => d.Quantity * d.Price);
                existing.CreatedAt = DateTime.Now;

                _context.InvoiceDetails.RemoveRange(existing.InvoiceDetails);

                foreach (var detail in invoice.InvoiceDetails)
                {
                    existing.InvoiceDetails.Add(new InvoiceDetail
                    {
                        Product = detail.Product,
                        Quantity = detail.Quantity,
                        Price = detail.Price
                    });
                }

                _context.SaveChanges();
            }
        }

        public void DeleteInvoice(int id)
        {
            var invoice = _context.Invoices.Find(id);
            if (invoice != null)
            {
                var details = _context.InvoiceDetails.Where(d => d.InvoiceId == id);
                _context.InvoiceDetails.RemoveRange(details);

                _context.Invoices.Remove(invoice);
                _context.SaveChanges();
            }
        }




    }
}
