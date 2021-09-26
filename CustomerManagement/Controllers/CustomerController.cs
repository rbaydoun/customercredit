using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.DAL;
using CustomerManagement.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerManagement.Controllers
{
  
  [Route("[controller]")]
  public class CustomerController : Controller
  {
    private readonly UnitOfWork unitOfWork;
    public CustomerController(UnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    // GET: customer
    /// <summary>
    /// Retrieve a list of all customers in the data store.
    /// </summary>
    /// <response code="200">Customers successfully patched.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpGet]
    public IEnumerable<Customer> Get()
    { 
      return unitOfWork.CustomerRepository.GetAll();
    }

    // GET customer/{id}
    /// <summary>
    /// Retrieve a specific custommer from the data store.
    /// </summary>
    /// <param name="id">The ID of the customer for which to retrieve the information.</param>
    /// <response code="200">Customer successfully retrieved.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpGet("{id}")]
    public Customer Get(long id)
    {
      return unitOfWork.CustomerRepository.GetById(id);
    }

    // POST customer
    /// <summary>
    /// Create a new customer and persists their information in the data store.
    /// </summary>
    /// <param name="model">JSON Body containing all required fields for customer creation</param>
    /// <response code="201">Customer successfully created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Post([FromBody] Customer model)
    {
      try
      {
        if (ModelState.IsValid)
        {
          unitOfWork.CustomerRepository.Insert(model);
          unitOfWork.Save();

          return StatusCode(StatusCodes.Status201Created, model);

        }
        else
        {
          return StatusCode(StatusCodes.Status400BadRequest, model);
        }  
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // PATCH customer/{id}
    /// <summary>
    /// Patch method to update fields for a specific customer.
    /// </summary>
    /// <param name="id">The ID of the customer to update.</param>
    /// <param name="patch">The JSON patch describing the information to update.</param>
    /// <response code="200">Customer successfully patched.</response>
    /// <response code="204">Customer doesn't exist.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public IActionResult Patch(long id, [FromBody] JsonPatchDocument<Customer> patch)
    {
      try
      {
        var customer = unitOfWork.CustomerRepository.GetById(id);

        if (customer != null)
        {
          patch.ApplyTo(customer, ModelState);
          unitOfWork.Save();

          return StatusCode(StatusCodes.Status200OK, customer);
        }
        return StatusCode(StatusCodes.Status204NoContent);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // DELETE customer/{id}
    /// <summary>
    /// Method to delete a specific customer profile.
    /// </summary>
    /// <param name="id">The ID of the customer to delete.</param>
    /// <response code="200">Customer successfully deleted.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(long id)
    {
      // Database will delete cards on cascade. The software shouldn't be aware
      // of that implementaion detail.
      try
      {
        unitOfWork.CustomerRepository.Delete(id);
        unitOfWork.Save();

        return StatusCode(StatusCodes.Status200OK);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // GET customer/{id}/card
    /// <summary>
    /// Validate the ownership of a credit card (described by "model") by a
    /// customer identified by "id".
    /// credit card described by the "model" JSON body.
    /// </summary>
    /// <param name="id">The ID of the customer for whom to verify ownership.</param>
    /// <param name="model">Description of the card.</param>
    /// <response code="200"></response>
    /// <response code="400">The input data is bad.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns>True: The customer owns the decribed card. False otherwise.</returns>
    [HttpGet("{id}/card")]
    public IActionResult Get(long id, [FromBody] Card model)
    {
      bool validCard = false;
      try
      { 
        if (ModelState.IsValid)
        {
          var customerCard = unitOfWork.CardRepository.GetByCustomerIdAndNumber(id, model.Number);
          if (customerCard != null)
          {
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

        return StatusCode(StatusCodes.Status400BadRequest, model);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // POST customer/{id}/card
    /// <summary>
    /// Add a new credit card to a customer.
    /// </summary>
    /// <param name="id">The ID of the customer for which to add the card.</param>
    /// <param name="model">Description of the card to add.</param>
    /// <response code="200">Card successfully created.</response>
    /// <response code="400">Card already exists.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpPost("{id}/card")]
    public IActionResult Post(long id, [FromBody] Card model)
    {
      try
      {
        string newNumber = model.Number;    // Used becasue the model will get encrypted.
        if (ModelState.IsValid)
        {
          var customerCard = unitOfWork.CardRepository.GetByCustomerIdAndNumber(id, newNumber);

          if (customerCard == null)
          {
            model.CustomerId = id;
            unitOfWork.CardRepository.Insert(model);
            unitOfWork.Save();

            var newCard = unitOfWork.CardRepository.GetByCustomerIdAndNumber(id, newNumber);
            return StatusCode(StatusCodes.Status201Created, newCard);
          }
        }

        return StatusCode(StatusCodes.Status400BadRequest, "Card already exists.");
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    // PATCH customer/{id}/card/{number}
    /// <summary>
    /// Update a specific card of a specified customer.
    /// </summary>
    /// <param name="id">The ID of the customer having ownership of the card.</param>
    /// <param name="number">The card number to update.</param>
    /// <param name="patch">The JSON patch</param>
    /// <response code="200">Card successfully patched.</response>
    /// <response code="204">Card doesn't exist.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpPatch("{id}/card/{number}")]
    public IActionResult Patch(long id, string number, [FromBody] JsonPatchDocument<Card> patch)
    {
      try
      {
        var customerCard = unitOfWork.CardRepository.GetByCustomerIdAndNumber(id, number);

        if (customerCard != null)
        {
          patch.ApplyTo(customerCard, ModelState);
          unitOfWork.CardRepository.Update(customerCard);
          unitOfWork.Save();

          var newCard = unitOfWork.CardRepository.GetCardById(customerCard.Id);
          return StatusCode(StatusCodes.Status200OK, customerCard);

        }
        return StatusCode(StatusCodes.Status204NoContent);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
      //try
      //{
      //  var card = dbContext.Cards
      //    .Where(c => c.CustomerId == id)
      //    .ToList()
      //    .First(c => protector.Unprotect(c.Number) == number);

      //  if (card != null)
      //  {
      //    patch.ApplyTo(card, ModelState);
      //    card.Number = protector.Protect(card.Number);
      //    card.Cvv = protector.Protect(card.Cvv);
      //    dbContext.SaveChanges();

      //    return StatusCode(StatusCodes.Status200OK, card);
      //  }
      //  else
      //  {
      //    return StatusCode(StatusCodes.Status204NoContent);
      //  }
      //}
      //catch (Exception ex)
      //{
      //  return StatusCode(StatusCodes.Status500InternalServerError, ex);
      //}
    }

    // DELETE customer/{id}/card/{number}
    /// <summary>
    /// Delete a credit card from a customer profile.
    /// </summary>
    /// <param name="id">The ID of the customer owning the card.</param>
    /// <param name="number">The card number to delete.</param>
    /// <response code="200">Card successfully deleted.</response>
    /// <response code="500">Internal server error.</response>
    /// <returns></returns>
    [HttpDelete("{id}/card/{number}")]
    public IActionResult Delete(long id, string number)
    {

      try
      {
        unitOfWork.CardRepository.DeleteByCustomerIdAndNumber(id, number);
        unitOfWork.Save();

        return StatusCode(StatusCodes.Status200OK);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        unitOfWork.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
