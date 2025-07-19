using RootsApp.Models;

namespace RootsApp.Services.Interfaces
{
    public interface IInvoiceService
    {
        IEnumerable<Invoice> GetAll();
        List<Invoice> GetAllByUserId(int userId);

        Invoice GetById(int id);
        bool CreateInvoice(Invoice invoice);

        void UpdateInvoice(Invoice invoice);
        void DeleteInvoice(int id);


    }
}
