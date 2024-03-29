﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Exceptions.Extensions;

public static class ProblemDetailsExtensions
{
    //burada once bir tane TProblemDetail gelecek
    //TProblemDetail 'in details bilgisini alacak ve serileştirecek
    public static string AsJson<TProblemDetail>(this TProblemDetail details)
        where TProblemDetail : ProblemDetails => JsonSerializer.Serialize(details);
}
