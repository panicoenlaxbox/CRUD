using NUnit.Framework;
using WebApplication1.Models;

namespace WebApplication1.Test
{
    public class EfCustomersRepositoryTests
    {
        [Test]
        public void GetAll_WithStronglyTypedOrderBy_DoesNotThrowAException()
        {
            //Arrange
            var repository = new EfCustomersRepository(new WebApplication1DbContext());
            //Act
            PaginatedResult<Customer> result = repository.GetAll<int>(1, 10, null, customer => customer.Id);
            //Assert
            Assert.Pass();
        }

        [Test]
        public void GetAll_WithDynamicOrderBy_DoesNotThrowAException()
        {
            //Arrange
            var repository = new EfCustomersRepository(new WebApplication1DbContext());
            //Act
            PaginatedResult<Customer> result = repository.GetAll(1, 10, null, "Id");
            //Assert
            Assert.Pass();
        }
    }

    public class CustomersServiceTests
    {
        [Test]
        public void GetAll_DoesNotThrowAException()
        {
            //Arrange
            var service = new CustomersService(new EfUnitOfWork(new WebApplication1DbContext()));
            //Act
            PaginatedResult<CustomerDto> result = service.GetAll(1, 10, "John Doe", "Name");
            //Assert
            Assert.Pass();
        }
    }
}
