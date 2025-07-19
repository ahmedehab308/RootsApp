using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsApp.Models;
using RootsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RootsApp.Models.ViewModel;

namespace RootsApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly RootsDbContext _context;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var invoices = _invoiceService.GetAllByUserId(userId.Value);
            return View(invoices);
        }

        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var invoice = new Invoice
            {
                UserId = userId.Value,
                InvoiceDetails = new List<InvoiceDetail>
                {
                    new InvoiceDetail()
                }
            };

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Invoice invoice)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            invoice.UserId = userId.Value;

            ModelState.Remove("User");
            ModelState.Remove("InvoiceDetails[0].Invoice");

            var keysToRemove = ModelState.Keys.Where(key =>
                key.Contains(".Invoice") || key.Equals("User")).ToList();

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

            if (invoice.InvoiceDetails != null)
            {
                invoice.InvoiceDetails = invoice.InvoiceDetails
                    .Where(d => !string.IsNullOrWhiteSpace(d.Product) && d.Quantity > 0 && d.Price > 0)
                    .ToList();
            }

            if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
            {
                ModelState.AddModelError("", "At least one product must be added.");
                return View(invoice);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool success = _invoiceService.CreateInvoice(invoice);
                    if (success)
                    {
                        TempData["Success"] = "Invoice created successfully.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating invoice: {ex.Message}");
                }
            }

            // Print validation errors for debugging
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(invoice);
        }

        public IActionResult Details(int id)
        {
            var invoice = _invoiceService.GetById(id);
            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        public IActionResult Edit(int id)
        {
            var invoice = _invoiceService.GetById(id);
            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Invoice invoice)
        {
            // Remove navigation properties from ModelState validation
            ModelState.Remove("User");

            // Remove Invoice navigation property from each InvoiceDetail
            for (int i = 0; i < invoice.InvoiceDetails.Count; i++)
            {
                ModelState.Remove($"InvoiceDetails[{i}].Invoice");
            }

            if (!ModelState.IsValid)
                return View(invoice);

            _invoiceService.UpdateInvoice(invoice);
            return RedirectToAction("Index");
        }

        // GET: Invoice/Delete/5
        public IActionResult Delete(int id)
        {
            var invoice = _invoiceService.GetById(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _invoiceService.DeleteInvoice(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
