using Core.CrossCuttingConcerns.Exceptions.Handlers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Exceptions;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next; //next burada invoke edilecek yani çalıştırılacak metodu temsil eder, aynmı zamanda requesttir.
	private readonly HttpExceptionHandler _httpExceptionHandler;

	public ExceptionMiddleware(RequestDelegate next)
	{
		_httpExceptionHandler = new HttpExceptionHandler();
		_next = next;
	}

	//butun kodlar bir metodun içerisinde gececek.
	//uygulamanın her yerine try catch yazmak yerıne burada bir tane yazayım ve tüm uygulama buradan geçsin
	public async Task Invoke(HttpContext context)
	{
		try
		{
			//burada apiden gelen isteği olduğu gibi çalıştır diyorum
			await _next(context);
		}
		catch (Exception exception)
		{
			await HandleExceptionAsync(context.Response, exception);
		}
	}

	private Task HandleExceptionAsync(HttpResponse response, Exception exception)
	{
		response.ContentType = "application/json";
		_httpExceptionHandler.Response = response;

		//buradaki HandleExceptionAsync --> ExceptionHandler class 'ındaki abstract olan metotlardan biri,
		//artık burada gönderdiğim exception hangi type 'ı karsılıyor ise
		return _httpExceptionHandler.HandleExceptionAsync(exception); 
	}
}
