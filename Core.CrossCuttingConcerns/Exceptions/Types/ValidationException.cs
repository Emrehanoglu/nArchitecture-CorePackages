using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Exceptions.Types;

public class ValidationException : Exception
{
	//hataları liste şeklinde vermeliyim
	public IEnumerable<ValidationExceptionModel> Errors { get; }

	public ValidationException()
		: base()
	{
		//ValidationException kullanılıyorsa önce boş bir liste oluştur
		Errors = Array.Empty<ValidationExceptionModel>();
	}

	public ValidationException(string? message)
		: base(message)
	{
		Errors = Array.Empty<ValidationExceptionModel>();
	}

	public ValidationException(string? message, Exception? innerException)
		: base(message, innerException)
	{
		Errors = Array.Empty<ValidationExceptionModel>();
	}

	public ValidationException(IEnumerable<ValidationExceptionModel> errors)
		: base(BuildErrorMessage(errors))
	{
		//hataları göndermiş ise listeye doldur. Bunu yaparken yardımcı BuildErrorMessage metodunu kullanıyorum
		Errors = errors;
	}

	private static string BuildErrorMessage(IEnumerable<ValidationExceptionModel> errors)
	{
		IEnumerable<string> arr = errors.Select(
			x => $"{Environment.NewLine} -- {x.Property}: {string.Join(Environment.NewLine, values: x.Errors ?? Array.Empty<string>())}"
		);
		return $"Validation failed: {string.Join(string.Empty, arr)}";
	}
}

public class ValidationExceptionModel
{
	public string? Property { get; set; } //hangi alanda hata var? Name,Date vs.
	public IEnumerable<string>? Errors { get; set; } //hangi hata veya hatalar var
}