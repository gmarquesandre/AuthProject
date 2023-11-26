using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthProject.Controllers
{
    public class MainController : ControllerBase
    {
        protected ICollection<string> Errors = new List<string>();

        protected ActionResult CustomResponse(object? result = null)
        {
            if (ValidOperation())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Messages", Errors.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                AddErrorToStack(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                AddErrorToStack(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ResponseResult responseResult)
        {
            ResponseHasErrors(responseResult);

            return CustomResponse();
        }

        protected bool ResponseHasErrors(ResponseResult responseResult)
        {
            if (responseResult == null || responseResult.Errors.Messages.Count == 0) return false;

            foreach (var errorMessage in responseResult.Errors.Messages)
            {
                AddErrorToStack(errorMessage);
            }

            return true;
        }

        protected bool ValidOperation()
        {
            return Errors.Count == 0;
        }

        protected void AddErrorToStack(string error)
        {
            Errors.Add(error);
        }

        protected void CleanErrors()
        {
            Errors.Clear();
        }
    }
}
