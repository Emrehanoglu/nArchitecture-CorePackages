using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Requests;

public class PageRequest
{
    public int PageIndex { get; set; } //kacıncı sayfa
    public int PageSize { get; set; } //her sayfada kac data
}
