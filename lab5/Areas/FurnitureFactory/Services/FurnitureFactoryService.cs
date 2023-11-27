using lab5.Areas.FurnitureFactory.Data;
using lab5.Areas.FurnitureFactory.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Entity;

namespace lab5.Areas.FurnitureFactory.Services
{
    public class FurnitureFactoryService
    {
        private AcmeDataContext _context;
        private IMemoryCache cache;
        public FurnitureFactoryService(AcmeDataContext context, IMemoryCache cache)
        {
            _context = context;
            this.cache = cache;
        }

        public IEnumerable<Order> GetOrders()
        {
            cache.TryGetValue("Orders", out IEnumerable<Order>? orders);

            if (orders is null)
            {
                orders = SetOrders();
            }
            return orders;
        }

        public IEnumerable<Order> SetOrders()
        {
            var orders = _context.Orders.Include(o => o.ResponsibleEmployee).Include(o => o.Customer)
                .ToList();
            cache.Set("Orders", orders, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(100000)));
            return orders;
        }

        public IEnumerable<Furniture> GetFurnitures()
        {
            cache.TryGetValue("Furnitures", out IEnumerable<Furniture>? furnitures);

            if (furnitures is null)
            {
                furnitures = SetFurnitures();
            }
            return furnitures;
        }

        public IEnumerable<Furniture> SetFurnitures()
        {
            var furnitures = _context.Furnitures
                .ToList();
            cache.Set("Furnitures", furnitures, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(100000)));
            return furnitures;
        }
        public IEnumerable<Customer> GetCustomers()
        {
            cache.TryGetValue("Customers", out IEnumerable<Customer>? customers);

            if (customers is null)
            {
                customers = SetCustomers();
            }
            return customers;
        }

        public IEnumerable<Customer> SetCustomers()
        {
            var customers = _context.Customers
                .ToList();
            cache.Set("Customers", customers, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(100000)));
            return customers;
        }
    }
}
