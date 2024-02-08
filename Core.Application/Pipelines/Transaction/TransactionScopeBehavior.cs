using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Core.Application.Pipelines.Transaction;

//bunun bir pipeline olabilmesi için <TRequset,TResponse> 'u verdim 
//aynı zamanda bunun bir IPipelineBehavior olması gerekiyor.
public class TransactionScopeBehavior<TRequset, TResponse> : IPipelineBehavior<TRequset, TResponse>
	where TRequset : IRequest<TResponse>, ITransactionalRequest
{
	public async Task<TResponse> Handle(TRequset request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		//async de calıstığım için bu özelliği enable ettim
		using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
		TResponse response;
		try
		{
			//gelen metodu çalıştır.
			response = await next();

			//try calıstığı için basarılı olmustur.
			transactionScope.Complete();
		}
		catch (Exception)
		{
			transactionScope.Dispose();
			throw;
		}
		return response;
	}
}
