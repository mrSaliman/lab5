using lab5.Areas.FurnitureFactory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab5.Areas.FurnitureFactory.Models;
using lab5.Areas.FurnitureFactory.ViewModels;
using lab5.Areas.FurnitureFactory.Filters;


using System.Drawing.Printing;
using lab5.Areas.FurnitureFactory.Services;

namespace lab5.Areas.FurnitureFactory.Controllers
{
    [Area("FurnitureFactory")]
    public class CustomerController : Controller
    {
        private readonly AcmeDataContext _context;
        private readonly int pageSize = 8; 


        public CustomerController(AcmeDataContext context)
        {
            _context = context;
        }

        // GET: FurnitureFactory/Customer
        public IActionResult Index(int page = 1)
        {
            CustomerViewModel customers;
            var customer = HttpContext.Session.Get<CustomerViewModel>("Customer");
            if (customer == null)
            {
                customer = new CustomerViewModel();
            }
            IQueryable<Customer> customersDbContext = _context.Customers;
            customersDbContext = Sort_Search(customersDbContext, customer.CompanyName ?? "", customer.RepresentativeLastName ?? "", customer.RepresentativeFirstName,
                customer.RepresentativeMiddleName ?? "", customer.PhoneNumber ?? "", customer.Address ?? "");
            // Разбиение на страницы
            var count = customersDbContext.Count();
            customersDbContext = customersDbContext.Skip((page - 1) * pageSize).Take(pageSize);


            customers = new CustomerViewModel
            {
                Customers = customersDbContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                CompanyName = customer.CompanyName,
                RepresentativeLastName = customer.RepresentativeLastName,
                RepresentativeFirstName = customer.RepresentativeFirstName,
                RepresentativeMiddleName = customer.RepresentativeMiddleName,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,

            };
            return View(customers);
        }
        [HttpPost]
        public IActionResult Index(CustomerViewModel customer)
        {

            HttpContext.Session.Set("Customer", customer);

            return RedirectToAction("Index");
        }


        // GET: FurnitureFactory/Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: FurnitureFactory/Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FurnitureFactory/Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CompanyName,RepresentativeLastName,RepresentativeFirstName,RepresentativeMiddleName,PhoneNumber,Address")] Customer customer)
        {
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                cache.SetCustomers();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: FurnitureFactory/Customer/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            var customer = cache.GetCustomers().FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: FurnitureFactory/Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CompanyName,RepresentativeLastName,RepresentativeFirstName,RepresentativeMiddleName,PhoneNumber,Address")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    cache.SetCustomers();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: FurnitureFactory/Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            var customer = cache.GetCustomers().FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: FurnitureFactory/Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customers'  is null.");
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            cache.SetCustomers();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
        private IQueryable<Customer> Sort_Search(IQueryable<Customer> customers, string CompanyName, string RepresentativeLastName, string RepresentativeFirstName,
             string RepresentativeMiddleName, string PhoneNumber, string Address)
        {

            customers = customers.Where(c => c.CompanyName.Contains(CompanyName ?? "")
            && c.RepresentativeLastName.Contains(RepresentativeLastName ?? "")
            && c.RepresentativeFirstName.Contains(RepresentativeFirstName ?? "")
            && c.RepresentativeMiddleName.Contains(RepresentativeMiddleName ?? "")
            && c.PhoneNumber.Contains(PhoneNumber ?? "")
            && c.Address.Contains(Address ?? ""));
            return customers;
        }
    }
}
