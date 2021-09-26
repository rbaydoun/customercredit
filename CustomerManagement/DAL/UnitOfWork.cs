using System;
using CustomerManagement.Encryption;
using Microsoft.AspNetCore.DataProtection;

namespace CustomerManagement.DAL
{
  public class UnitOfWork : IDisposable
  {
    private readonly CustomerManagementContext dbContext;
    private readonly IDataProtector protector;
    private readonly CustomerRepository customerRepository;
    private readonly CardRepository cardRepository;

    public UnitOfWork(CustomerManagementContext dbContext, IDataProtectionProvider dataProtectionProvider)
    {
      this.dbContext = dbContext;
      this.protector = dataProtectionProvider
        .CreateProtector(DataProtectionPurposeStrings.CreditCardInformation);
    }

    /// <summary>
    /// Return the instance of the Customer Repository.
    /// </summary>
    public CustomerRepository CustomerRepository
    {
      get { return this.customerRepository ?? new CustomerRepository(dbContext); }
    }

    /// <summary>
    /// Get the instance of the Card Repository.
    /// </summary>
    public CardRepository CardRepository
    {
      get { return this.cardRepository ?? new CardRepository(dbContext, protector); }
    }

    /// <summary>
    /// Save contxt to the data store.
    /// </summary>
    public void Save()
    {
      dbContext.SaveChanges();
    }

    // Dispose of DB context.
    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          dbContext.Dispose();
        }
      }
      this.disposed = true;
    }
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
