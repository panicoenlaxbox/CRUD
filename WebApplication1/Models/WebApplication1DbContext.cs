using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplication1.Models
{
    public class WebApplication1DbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customers").Property(customer => customer.Name).HasMaxLength(6).IsRequired();
            modelBuilder.Entity<Address>().ToTable("Addresses");
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        [RegularExpression(@"^s\w{1,5}$")]
        public string Name { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }

    public class Address
    {
        public Customer Customer { get; set; }
        public int Id { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }
}