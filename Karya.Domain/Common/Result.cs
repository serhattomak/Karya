using System.Net;
using System.Text.Json.Serialization;

namespace Karya.Domain.Common;

public class Result<T>
{
	public T? Data { get; set; }
	public List<string>? ErrorMessage { get; set; }
	[JsonIgnore] public bool IsSuccess => ErrorMessage == null || ErrorMessage.Count == 0;
	[JsonIgnore] public bool IsFailure => !IsSuccess;
	[JsonIgnore] public HttpStatusCode Status { get; set; }
	[JsonIgnore] public string? UrlAsCreated { get; set; }
	public static Result<T> Success(T data, HttpStatusCode status = HttpStatusCode.OK)
	{
		return new Result<T>
		{
			Data = data,
			Status = status
		};
	}
	public static Result<T> SuccessAsCreated(T data, string urlAsCreated)
	{
		return new Result<T>
		{
			Data = data,
			Status = HttpStatusCode.Created,
			UrlAsCreated = urlAsCreated
		};
	}
	public static Result<T> Failure(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
	{
		return new Result<T>
		{
			ErrorMessage = [errorMessage],
			Status = status
		};
	}

	public static Result<T> Failure(List<string> errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
	{
		return new Result<T>
		{
			ErrorMessage = errorMessage,
			Status = status
		};
	}
}

public class Result
{
	public List<string>? ErrorMessage { get; set; }
	[JsonIgnore] public bool IsSuccess => ErrorMessage == null || ErrorMessage.Count == 0;
	[JsonIgnore] public bool IsFailure => !IsSuccess;
	[JsonIgnore] public HttpStatusCode Status { get; set; }
	public static Result Success(HttpStatusCode status = HttpStatusCode.OK)
	{
		return new Result()
		{
			Status = status
		};
	}

	public static Result Failure(List<string> errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
	{
		return new Result()
		{
			ErrorMessage = errorMessage,
			Status = status
		};
	}

	public static Result Failure(string errorMessage, HttpStatusCode status = HttpStatusCode.BadRequest)
	{
		return new Result()
		{
			ErrorMessage = [errorMessage],
			Status = status
		};
	}
}