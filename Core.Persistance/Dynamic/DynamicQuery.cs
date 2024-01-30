using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Dynamic;

public class DynamicQuery
{
    public IEnumerable<Sort>? Sort { get; set; } //sort değeri list olarak yaptım, birden fazla sort değeri olabilir
    public Filter? Filter { get; set; } //birden fazla filter 'da olabilir ama bunu list olarak yapmadım
                                        //cunku Filter kendi içerisinde List olarak filter değerleri alabiliyor.
                                        //yani bu kısım tek bir filtre anlamına gelmiyor, iç içe filtre olmus oluyor
    public DynamicQuery()
    {
            
    }
    public DynamicQuery(IEnumerable<Sort>? sort, Filter? filter)
    {
        Filter = filter;
        Sort = sort;
    }
}
