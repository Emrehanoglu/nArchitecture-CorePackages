using Core.CrossCuttingConcerns.Exceptions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Exceptions.Handlers;

public abstract class ExceptionHandler
{
	//HandleExceptionAsync cağırıldığında gelen Exception type 'ına göre aşağıdaki switch 'lerden birine girecek
	//BusinessException gibi AuthorizationException,NotFoundException,ValidationException... gibi Exception 'lar olabilir.
	public Task HandleExceptionAsync(Exception exception) => 
		exception switch
		{
			BusinessException businessException => HandleException(businessException), //gelen Exception, Business type 'ında ise
			_ => HandleException(exception) //gelen Exception switch içerisindeki Type 'lardan birini karşılamıyor ise
		};

	//hangi ortam class 'ı ExceptionHandler 'ı inherit ederse aşağıdaki class 'ı kullanarak hata yönetimi yapabilecek.
	protected abstract Task HandleException(BusinessException businessException); //gelen Exception, Business ise burası calısacak
	protected abstract Task HandleException(Exception exception); //gelen Exception herhangi bir Type 'ı karsılamıyor ise burası calısacak
}