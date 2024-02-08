using Core.CrossCuttingConcerns.Exceptions.Types;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationException = Core.CrossCuttingConcerns.Exceptions.Types.ValidationException;

namespace Core.Application.Pipelines.Validation;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	//burası CreateBrandCommandValidator 'daki AbstractValidator<CreateBrandCommand> yapıya uyuyor.
	//AbstractValidator aynı zamanda bir IValidator 'dur.
	//CreateBrandCommand aynı zamanda TRequest 'dir.
	//bir requestin yanı commandin validator 'larına ulasacağım...
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		ValidationContext<object> context = new(request);

		//bir nesnedeki tüm validasyon hatalarını alıp bir listeye ekleyip kullanıcıya vereceğim.
		IEnumerable<ValidationExceptionModel> errors = _validators
			.Select(validator => validator.Validate(context))
			.SelectMany(result => result.Errors) //birden fazla hata olabilir
			.Where(failure => failure != null) //bir failure durum var ise
			.GroupBy(
			keySelector: p => p.PropertyName,
			resultSelector: (propertyName, errors) =>
			new ValidationExceptionModel { Property = propertyName, Errors = errors.Select(e => e.ErrorMessage) }
			).ToList();

		if (errors.Any())
			throw new ValidationException(errors); //hata var ise fırlat
		TResponse response = await next();
		return response; //hata yok ise Command 'i çalıştır
	}
}
