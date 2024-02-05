using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Exceptions.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
	private HttpResponse? _response;
	public HttpResponse Response
	{
		get => _response ?? throw new ArgumentNullException(nameof(_response));
		set => _response = value;
	}
	protected override Task HandleException(BusinessException businessException)
	{
		Response.StatusCode = StatusCodes.Status400BadRequest;

		//bir iş kuralı problemi oluştuğunda JSON formatında bunu dönmek istiyorum
		string details = new BusinessProblemDetails(businessException.Message).AsJson(); //JSON formatında olması lazım

		return Response.WriteAsync(details);
	}

	protected override Task HandleException(Exception exception)
	{
		Response.StatusCode = StatusCodes.Status500InternalServerError;

		//bir iş kuralı problemi oluştuğunda JSON formatında bunu dönmek istiyorum
		string details = new InternalServerErrorProblemDetails(exception.Message).AsJson(); //JSON formatında olması lazım

		return Response.WriteAsync(details);
	}
}