using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Repositories;

public interface IEntityTimestamps
{
	DateTime CreatedDate { get; set; } //bu nesne ne zaman olusturuldu
	DateTime? UpdatedDate { get; set; } //bu nesne ne zaman güncellendi, nesne ilk olustuğunda nullable olabilir
	DateTime? DeletedDate { get; set; } //bu nesne ne zaman silindi, nesne ilk olustuğunda nullable olabilir
}
