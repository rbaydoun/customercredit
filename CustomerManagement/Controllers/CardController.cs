using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManagement.Datastore;
using CustomerManagement.Datastore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerManagement.Controllers
{

  [Route("[controller]")]
  public class CardController : Controller
  {

    // PATCH card/{number}
    [HttpPatch("{number}")]
    public IActionResult Put(string number, [FromBody] JsonPatchDocument<Card> patch)
    {
      using var db = new CustomerManagementContext();

      try
      {
        var card = db.Cards.FirstOrDefault(card => card.Number == number);
        if (card != null)
        {
          patch.ApplyTo(card, ModelState);
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

    // DELETE card/{number}
    [HttpDelete("{number}")]
    public IActionResult Delete(string number)
    {
      // Database will delete cards on cascade. The software shouldn't be aware
      // of that implementaion detail.
      using var db = new CustomerManagementContext();

      try
      {
        var card = new Card() { Number = number };
        db.Cards.Attach(card);
        db.Cards.Remove(card);
        db.SaveChanges();

        return StatusCode(StatusCodes.Status200OK);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }
  }
}
