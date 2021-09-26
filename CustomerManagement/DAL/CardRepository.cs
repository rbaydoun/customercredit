using System;
using System.Collections.Generic;
using System.Linq;
using CustomerManagement.Encryption;
using CustomerManagement.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.DAL
{
  public class CardRepository
  {
    private readonly IDataProtector protector;
    private readonly CustomerManagementContext dbContext;
    
    public CardRepository(
      CustomerManagementContext dbContext,
      IDataProtectionProvider dataProtectionProvider)
    {
      this.dbContext = dbContext;
      this.protector = dataProtectionProvider
        .CreateProtector(DataProtectionPurposeStrings.CreditCardInformation);
    }

    public void Insert(Card model)
    {
      //var existingCard = GetByCustomerIdAndNumber(model.CustomerId, model.Number);
      //if (existingCard == null)
      //{
        var protectedNumber = protector.Protect(model.Number);
        var protectedCvv = protector.Protect(model.Cvv);

        model.Number = protectedNumber;
        model.Cvv = protectedCvv;
        dbContext.Cards.Add(model);
      //}
    }

    /// <summary>
    /// Retrieve a credit card entry by number, owned by a customer.
    /// </summary>
    /// <param name="id">The Id of the customer owning the card.</param>
    /// <param name="number">The number of the card to remove.</param>
    /// <returns></returns>
    public Card GetByCustomerIdAndNumber(long id, string number)
    {
      var encryptedCard = dbContext.Cards
        .Where(c => c.CustomerId == id)
        .ToList()
        .First(c => protector.Unprotect(c.Number) == number);

      if (encryptedCard != null)
      {
        var unprotectedNumber = protector.Unprotect(encryptedCard.Number);
        var unprotectedCvv = protector.Unprotect(encryptedCard.Cvv);

        return new Card()
        {
          Number = unprotectedNumber,
          Type = encryptedCard.Type,
          ExpiryDate = encryptedCard.ExpiryDate,
          Cvv = unprotectedCvv,
          CustomerId = encryptedCard.CustomerId
        };
      }
      return null;
    }

    /// <summary>
    /// Updates a card entry in the data store.
    /// </summary>
    /// <param name="card">The card entity to update.</param>
    public void Update(Card card)
    {
      card.Number = protector.Protect(card.Number);
      card.Cvv = protector.Protect(card.Cvv);

      dbContext.Cards.Attach(card);
      dbContext.Entry(card).State = EntityState.Modified;
    }

    /// <summary>
    /// Delete a specific card owned by a customer.
    /// </summary>
    /// <param name="id">The customer owning the card.</param>
    /// <param name="number">The number of the credit card to delete.</param>
    public void DeleteByCustomerIdAndNumber(long id, string number)
    {
      var cardToDelete = GetByCustomerIdAndNumber(id, number);

      if (cardToDelete != null)
      {
        dbContext.Cards.Remove(cardToDelete);
      }
    }
  }
}
