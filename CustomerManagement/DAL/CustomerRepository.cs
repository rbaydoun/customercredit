using System;
using System.Collections.Generic;
using System.Linq;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.DAL
{
  public class CustomerRepository
  {
    private readonly CustomerManagementContext dbContext;
    
    public CustomerRepository(CustomerManagementContext dbContext)
    {
      this.dbContext = dbContext;
    }

    /// <summary>
    /// Insert a new customer into the data store.
    /// </summary>
    /// <param name="model">The customer model to insert.</param>
    public void Insert(Customer model)
    {
      dbContext.Customers.Add(model);
    }

    /// <summary>
    /// Get all customers from the data store.
    /// </summary>
    /// <returns>List of customers.</returns>
    public IEnumerable<Customer> GetAll()
    {
      return dbContext.Customers.ToList();
    }

    /// <summary>
    /// Get a single customer from the data store.
    /// </summary>
    /// <param name="id">The ID identifying the requested customer.</param>
    /// <returns>Customer, if exists. Null otherwise.</returns>
    public Customer GetById(long id)
    {
      return dbContext.Customers.Find(id);

    }

    /// <summary>
    /// Update a customer.
    /// </summary>
    /// <param name="customer">The customer entity containing the update.</param>
    public void Update(Customer customer)
    {
      dbContext.Customers.Attach(customer);
      dbContext.Entry(customer).State = EntityState.Modified;
    }

    /// <summary>
    /// Delete a customer from the datastore.
    /// </summary>
    /// <param name="id">The ID identifying the customer to delete.</param>
    public void Delete(long id)
    {
      var customerToDelete = dbContext.Customers.Find(id);
      Delete(customerToDelete);
    }

    /// <summary>
    /// Delete a customer from the datastore.
    /// </summary>
    /// <param name="customer">The customer entity to delete.</param>
    public void Delete(Customer customer)
    {
      if (dbContext.Entry(customer).State == EntityState.Detached)
      {
        dbContext.Customers.Attach(customer);
      }
      dbContext.Customers.Remove(customer);
    }
  }
}
