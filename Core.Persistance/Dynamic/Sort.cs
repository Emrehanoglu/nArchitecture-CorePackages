using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Dynamic;

public class Sort
{
    public string Field { get; set; } //sıralamayı uygulayacağım filter alanı bilgisi
    public string Dir { get; set; } //Direction --> asc mi desc mi bir sıralama olacak bilgisi

    public Sort()
    {
        Field = string.Empty;
        Dir = string.Empty;
    }
    public Sort(string field, string dir)
    {
        Field = field;
        Dir = dir;
    }
}
