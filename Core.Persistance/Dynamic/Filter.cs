using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Dynamic;

public class Filter
{
    public string Field { get; set; } //filtre hangi alan olacak yani araba markasımı, yakıt cinsimi vs.
    public string? Value { get; set; }
    public string Operator { get; set; } //filtre alanının operatoru nedir, yani string bir ifade için, eşitti veya içinde bulunan mı
                                          //integer bir ifade için büyüktür küçüktür mü vs.
    public string? Logic{ get; set; } //birden fazla filtre olduğu zaman and,or ilişkisi kurmak için
    public IEnumerable<Filter>? Filters { get; set;} // filtre listesi

    //yardımcı ctor
    public Filter()
    {
        Field = string.Empty;
        Operator = string.Empty;
    }
	//yardımcı ctor
    //oprator ibaresi C# mevcutta var olan bir keyword fakat ben kendi prop 'um için olan opaerator 'u ifade etmek istiyorsam basınsa @ koymalıyım
	public Filter(string field, string @operator)
    {
        Field = field;
        Operator = @operator;
    }
}