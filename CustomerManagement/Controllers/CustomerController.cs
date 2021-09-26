using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.Datastore;
using CustomerManagement.Datastore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerManagement.Controllers
{
  [Route("[controller]")]
  public class CustomerController : Controller
  {
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

    // POST card
    [HttpPost("{id}/card")]
    public IActionResult Post(long id, [FromBody] Card model)
    {
      using var db = new CustomerManagementContext();
      try
      {
        var newCard = db.Cards.Add(model);
        newCard.Entity.CustomerId = id;
        db.SaveChanges();
        return StatusCode(StatusCodes.Status201Created, newCard.Entity);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }




    // PATCH customer/{id}
    [HttpPatch("{id}")]
    public IActionResult Put(long id, [FromBody] JsonPatchDocument<Customer> patch)
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
    public IActionResult Delete(int id)
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
    public IActionResult Get(long id, [FromBody] Card card)
    {
      using var db = new CustomerManagementContext();

      try
      {
        var customerCard = db.Cards.Find(card.Number);

        bool validCard = false;
        if (customerCard != null)
        {
          // Run validation.
          if ((customerCard.Cvv == card.Cvv) &&
             (customerCard.ExpiryDate == card.ExpiryDate) &&
             (customerCard.Type == card.Type) &&
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
  }
}
