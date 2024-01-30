using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Paging;

public class Paginate<T>
{
    public Paginate()
    {
        Items = Array.Empty<T>(); //ilk etapta boş olabilir   
    }

    public int Size { get; set; } //sayfada kaç data var
    public int Index { get; set; } //hangi sayfadayız
    public int Count { get; set; } //toplam kayıt sayısı
    public int Pages  { get; set; } //toplam sayfa sayısı
    public IList<T> Items { get; set; } //listeleyeceğim elemanlar
    public bool HasPrevious => Index > 0; //onceki sayfa var mı
    public bool HasNext => Index+1 > Pages; //sonraki sayfa var mı
}