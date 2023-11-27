using lab5.Areas.FurnitureFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace lab5.Areas.FurnitureFactory.Data;

public partial class AcmeDataContext : DbContext
{
    public AcmeDataContext()
    {
    }

    public AcmeDataContext(DbContextOptions<AcmeDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Furniture> Furnitures { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
}
