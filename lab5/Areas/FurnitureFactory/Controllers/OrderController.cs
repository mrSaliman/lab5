using lab5.Areas.FurnitureFactory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab5.Areas.FurnitureFactory.Models;
using lab5.Areas.FurnitureFactory.Filters;
using lab5.Areas.FurnitureFactory.ViewModels;
using System.Drawing.Printing;
using lab5.Areas.FurnitureFactory.Services;

namespace lab5.Areas.FurnitureFactory.Controllers
{
    [Area("FurnitureFactory")]
    public class OrderController : Controller
    {
        private readonly AcmeDataContext _context;
        private readonly int pageSize = 8;


        public OrderController(AcmeDataContext context)
        {
            _context = context;
        }

        // GET: FurnitureFactory/Order
        public IActionResult Index(int page = 1)
        {
            OrderViewModel orders;
            var order = HttpContext.Session.Get<OrderViewModel>("Order");
            if (order == null)
            {
                order = new OrderViewModel();
            }
            IQueryable<Order> ordersDbContext = _context.Orders;
            ordersDbContext = Sort_Search(ordersDbContext,order.OrderDate, order.SpecialDiscount, order.IsCompleted, order.ResponsibleEmployeeFirstName
                ,order.CustomerCompanyName);
            // Разбиение на страницы
            var count = ordersDbContext.Count();
            ordersDbContext = ordersDbContext.Skip((page - 1) * pageSize).Take(pageSize);


            orders = new OrderViewModel
            {
                Orders = ordersDbContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                OrderDate = order.OrderDate,
                SpecialDiscount = order.SpecialDiscount,
                IsCompleted = order.IsCompleted,
                ResponsibleEmployeeFirstName = order.ResponsibleEmployeeFirstName,
                CustomerCompanyName = order.CustomerCompanyName

            };
            return View(orders);
        }
        [HttpPost]
        public IActionResult Index(OrderViewModel order)
        {

            HttpContext.Session.Set("Order", order);

            return RedirectToAction("Index");
        }

        // GET: FurnitureFactory/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ResponsibleEmployee)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: FurnitureFactory/Order/Create
        public IActionResult Create()
        {

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName");
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName");
            return View();
        }

        // POST: FurnitureFactory/Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,CustomerId,SpecialDiscount,IsCompleted,ResponsibleEmployeeId")] Order order)
        {
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                cache.SetOrders();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", order.CustomerId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", order.ResponsibleEmployeeId);
            return View(order);
        }

        // GET: FurnitureFactory/Order/Edit/5
        public IActionResult Edit(int? id)
        {

            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            var order =  cache.GetOrders().FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", order.CustomerId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", order.ResponsibleEmployeeId);
            return View(order);
        }

        // POST: FurnitureFactory/Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,CustomerId,SpecialDiscount,IsCompleted,ResponsibleEmployeeId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    cache.SetOrders();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", order.CustomerId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", order.ResponsibleEmployeeId);
            return View(order);
        }

        // GET: FurnitureFactory/Order/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            var order = _context.Orders.Include(o => o.Customer).Include(o => o.ResponsibleEmployee)
                .FirstOrDefault(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: FurnitureFactory/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cache = HttpContext.RequestServices.GetService<FurnitureFactoryService>();

            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            _context.SaveChanges();
            cache.SetOrders();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
        private IQueryable<Order> Sort_Search(IQueryable<Order> orders, DateTime? OrderDate, decimal SpecialDiscount,
            bool IsCompleted, string ResponsibleEmployeeFirstName, string CustomerCompanyName)
        {

            orders = orders.Include(o => o.Customer).Include(o => o.ResponsibleEmployee)
            .Where(c => (c.OrderDate.Date == OrderDate || OrderDate == new DateTime() || OrderDate == null)
            && (c.SpecialDiscount == SpecialDiscount || SpecialDiscount == 0)
            && c.ResponsibleEmployee.FirstName.Contains(ResponsibleEmployeeFirstName ?? "")
            && (c.IsCompleted == IsCompleted)
            && c.Customer.CompanyName.Contains(CustomerCompanyName ?? ""));
            return orders;
        }
    }
}
