using RootsApp.Models;

namespace RootsApp.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        IEnumerable<Invoice> GetAll();
        List<Invoice> GetAllByUserId(int userId);

        Invoice GetById(int id);
        void Add(Invoice invoice);
        void UpdateInvoice(Invoice invoice);
        void DeleteInvoice(int id);


        void Save();
    }
}
