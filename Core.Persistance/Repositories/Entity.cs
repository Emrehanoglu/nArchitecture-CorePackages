using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Repositories;

public class Entity<TId>
{
    public TId Id { get; set; }
    public DateTime CreatedDate { get; set; } //bu nesne ne zaman olusturuldu
    public DateTime? UpdatedDate { get; set; } //bu nesne ne zaman güncellendi, nesne ilk olustuğunda nullable olabilir
    public DateTime? DeletedDate { get; set; } //bu nesne ne zaman silindi, nesne ilk olustuğunda nullable olabilir

    //farklı kullanımlar için ctor desteği getiriyorum
    public Entity()
    {
        Id = default; //ıd hiç verilmemişte default olarak tipi ne ise Id o olsun, orneğin int tipindeki Id için default deger sıfır.       
    }
    public Entity(TId id)
    {
        Id = id; //TId türünde bir id değeri gelirse Id ye eşitledim.
    }
}
