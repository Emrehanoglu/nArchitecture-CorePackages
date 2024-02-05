using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Exceptions.Types;

public class BusinessException:Exception
{
    public BusinessException() { }

    public BusinessException(string? message) :base(message) { } //base ile Exception 'a da message 'ı gönderdim, message null 'da olabilir.

	public BusinessException(string? message, Exception? innerexception) : base(message,innerexception)	{ }
}
