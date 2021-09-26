using System;
using CustomerManagement.DAL;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace CustomerManagement.Tests
{
  public class CustomerRepository_Tests
  {
    [Fact]
    public void Insert_DbSetAddInvoked()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      var customerRepository = new CustomerRepository(dbContextMock.Object);
      var model = new Customer();

      // Act
      customerRepository.Insert(model);

      // Assert
      dbSetCustomerMock.Verify(x => x.Add(model), Times.Exactly(1));
    }

    //[Fact]
    //public void GetAll_DbSetToListInvoked()
    //{
    //  // Arrange
    //  var dbContextMock = new Mock<CustomerManagementContext>();
    //  var dbSetCustomerMock = new Mock<DbSet<Customer>>();
    //  dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
    //  var customerRepository = new CustomerRepository(dbContextMock.Object);

    //  // Act
    //  customerRepository.GetAll();

    //  // Assert
    //  dbSetCustomerMock.Verify(x => , Times.Exactly(1));
    //}

    [Fact]
    public void GetById_DbSetFindInvoked()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      var customerRepository = new CustomerRepository(dbContextMock.Object);
      long id = 1;

      // Act
      customerRepository.GetById(id);

      // Assert
      dbSetCustomerMock.Verify(x => x.Find(id), Times.Exactly(1));
    }

    [Fact]
    public void GetById_GetCustomerMatchingId()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      var customerRepository = new CustomerRepository(dbContextMock.Object);

      long id = 1;
      var customer = new Customer() { Id = id };
      dbSetCustomerMock.Setup(x => x.Find(id)).Returns(customer);

      // Act
      var result = customerRepository.GetById(id);

      // Assert
      Assert.Equal(id, result.Id);
    }

    [Fact]
    public void GetById_GetNullWhenNoMatchingId()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      var customerRepository = new CustomerRepository(dbContextMock.Object);
      long id = 1;

      // Act
      var result = customerRepository.GetById(id);

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public void Update_AttachAndEntryInvoked()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();


      //// Broken --> Fix me.
      ////dbContextMock.Setup(x => x.Entry(customer)).Returns(---);
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      var customerRepository = new CustomerRepository(dbContextMock.Object);
      var customer = new Customer();

      // Act
      //customerRepository.Update(customer);

      // Assert
      //dbContextMock.Verify(x => x.Entry(customer), Times.Exactly(1));
      //dbSetCustomerMock.Verify(x => x.Attach(customer), Times.Exactly(1));
    }

    [Fact]
    public void Delete_DeleteWithId()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();

      // Broken --> Fix me.
      //dbContextMock.Setup(x => x.Entry(customer)).Returns(---);
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      long id = 1;
      var customer = new Customer() { Id = id };
      dbSetCustomerMock.Setup(x => x.Find(id)).Returns(customer);
      var customerRepository = new CustomerRepository(dbContextMock.Object);

      // Act
      //customerRepository.Delete(id);

      // Assert
      //dbContextMock.Verify(x => x.Entry(customer), Times.Exactly(1));
      //dbSetCustomerMock.Verify(x => x.Attach(customer), Times.Exactly(1));
    }

    [Fact]
    public void Delete_DeleteWithCustomer()
    {
      // Arrange
      var dbContextMock = new Mock<CustomerManagementContext>();
      var dbSetCustomerMock = new Mock<DbSet<Customer>>();

      // Broken --> Fix me.
      //dbContextMock.Setup(x => x.Entry(customer)).Returns(---);
      dbContextMock.Setup(x => x.Customers).Returns(dbSetCustomerMock.Object);
      var customer = new Customer();
      var customerRepository = new CustomerRepository(dbContextMock.Object);

      // Act
      //customerRepository.Delete(customer);

      // Assert
      //dbSetCustomerMock.Verify(x => x.Remove(customer), Times.Exactly(1));
    }
  }
}
