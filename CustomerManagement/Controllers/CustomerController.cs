using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.Datastore;
using CustomerManagement.Datastore.Encryption;
using CustomerManagement.Datastore.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerManagement.Controllers
{
  
  [Route("[controller]")]
  public class CustomerController : Controller
  {
    private readonly IDataProtector protector;

    public CustomerController(IDataProtectionProvider dataProtectionProvider)
    {
      protector = dataProtectionProvider
        .CreateProtector(DataProtectionPurposeStrings.CreditCardInformation);
    }

    // GET: customer
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
      using var db = new CustomerManagementContext();
      return db.Customers.ToList();
    }

    // GET customer/{id}
    [HttpGet("{id}")]
    public Customer Get(long id)
    {
      using var db = new CustomerManagementContext();
      var customer = db.Customers.Find(id);
      return customer;
    }

    // POST customer
    [HttpPost]
    public IActionResult Post([FromBody] Customer model)
    {
      using var db = new CustomerManagementContext();
      try
      {
        var newCustomer = db.Customers.Add(model);
        db.SaveChanges();

        return StatusCode(StatusCodes.Status201Created, model);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // PATCH customer/{id}
    [HttpPatch("{id}")]
    public IActionResult Patch(long id, [FromBody] JsonPatchDocument<Customer> patch)
    {
      using var db = new CustomerManagementContext();

      try
      {
        var customer = db.Customers.FirstOrDefault(customer => customer.Id == id);
        if (customer != null)
        {
          patch.ApplyTo(customer, ModelState);
          db.SaveChanges();

          return StatusCode(StatusCodes.Status200OK, customer);
        }
        else
        {
          return StatusCode(StatusCodes.Status204NoContent);
        }
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // DELETE customer/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(long id)
    {
      // Database will delete cards on cascade. The software shouldn't be aware
      // of that implementaion detail.
      using var db = new CustomerManagementContext();

      try
      {
        var customer = new Customer() { Id = id };
        db.Customers.Attach(customer);
        db.Customers.Remove(customer);
        db.SaveChanges();

        return StatusCode(StatusCodes.Status200OK);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // GET customer/{id}/card
    [HttpGet("{id}/card")]
    public IActionResult Get(long id, [FromBody] Card model)
    {
      using var db = new CustomerManagementContext();

      try
      {
        var customerCard = db.Cards
          .Where(c => c.CustomerId == id)
          .ToList()
          .First(c => protector.Unprotect(c.Number) == model.Number);

        bool validCard = false;
        if (customerCard != null)
        {
          var unprotectedNumber = protector.Unprotect(customerCard.Number);
          var unprotectedCvv = protector.Unprotect(customerCard.Cvv);

          // Run validation.
          if ((customerCard.Cvv == model.Cvv) &&
             (customerCard.ExpiryDate == model.ExpiryDate) &&
             (customerCard.Type == model.Type) &&
             (customerCard.CustomerId == id))
          {
            validCard = true;
          }
        }

        return StatusCode(StatusCodes.Status200OK, validCard);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // POST card
    [HttpPost("{id}/card")]
    public IActionResult Post(long id, [FromBody] Card model)
    {
      using var db = new CustomerManagementContext();
      try
      {
        var card = db.Cards
          .Where(c => c.CustomerId == id)
          .ToList()
          .First(c => protector.Unprotect(c.Number) == model.Number);

        if (card == null)
        {
          model.Number = protector.Protect(model.Number);
          model.Cvv = protector.Protect(model.Cvv);
          model.CustomerId = id;
          db.Cards.Add(model);
          db.SaveChanges();

          return StatusCode(StatusCodes.Status201Created, model);
        }
        else
        {
          return StatusCode(StatusCodes.Status400BadRequest);
        }

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // PATCH customer/{id}/card/{number}
    [HttpPatch("{id}/card/{number}")]
    public IActionResult Patch(long id, string number, [FromBody] JsonPatchDocument<Card> patch)
    {
      using var db = new CustomerManagementContext();

      try
      {
        var card = db.Cards
          .Where(c => c.CustomerId == id)
          .ToList()
          .First(c => protector.Unprotect(c.Number) == number);

        if (card != null)
        {
          patch.ApplyTo(card, ModelState);
          card.Number = protector.Protect(card.Number);
          card.Cvv = protector.Protect(card.Cvv);
          db.SaveChanges();

          return StatusCode(StatusCodes.Status200OK, card);
        }
        else
        {
          return StatusCode(StatusCodes.Status204NoContent);
        }
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // DELETE customer/{id}/card/{number}
    [HttpDelete("{id}/card/{number}")]
    public IActionResult Delete(long id, string number)
    {
      // Database will delete cards on cascade. The software shouldn't be aware
      // of that implementaion detail.
      using var db = new CustomerManagementContext();

      try
      {
        var card = db.Cards
          .Where(c => c.CustomerId == id)
          .ToList()
          .First(c => protector.Unprotect(c.Number) == number);

        if (card != null)
        {
          db.Cards.Remove(card);
          db.SaveChanges();
        }

        return StatusCode(StatusCodes.Status200OK);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }
  }
}
