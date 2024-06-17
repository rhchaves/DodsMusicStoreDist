using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DMS.Identity.API.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
        protected ICollection<string> Errors = new List<string>();

        protected ActionResult CustomResponse(object result = null)
        {
            if (ValidatedOperation())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Errors.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                AddErrorProcessing(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected bool ValidatedOperation()
        {
            return !Errors.Any();
        }

        protected void AddErrorProcessing(string error)
        {
            Errors.Add(error);
        }

        protected void ClearErrorsProcessing()
        {
            Errors.Clear();
        }
    }
}
