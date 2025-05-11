using Karya.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomBaseController : ControllerBase
	{
		[NonAction]
		public IActionResult CreateActionResult<T>(Result<T> result)
		{
			return result.Status switch
			{
				HttpStatusCode.NotFound => NotFound(result),
				HttpStatusCode.BadRequest => BadRequest(result),
				HttpStatusCode.Unauthorized => Unauthorized(result),
				HttpStatusCode.Created => Created(result.UrlAsCreated, result),
				HttpStatusCode.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, result),
				_ => new ObjectResult(result) { StatusCode = result.Status.GetHashCode() }
			};
		}
		[NonAction]
		public IActionResult CreateActionResult(Result result)
		{
			return result.Status switch
			{
				HttpStatusCode.NoContent => new ObjectResult(result) { StatusCode = result.Status.GetHashCode() },
				HttpStatusCode.NotFound => NotFound(result),
				HttpStatusCode.BadRequest => BadRequest(result),
				HttpStatusCode.Unauthorized => Unauthorized(result),
				HttpStatusCode.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, result),
				_ => new ObjectResult(result) { StatusCode = result.Status.GetHashCode() }
			};
		}
	}
}
