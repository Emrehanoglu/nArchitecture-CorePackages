using Core.Persistance.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Response;

public class GetListResponse<T>: BasePageableModel
{
	private IList<T> _items;

	public IList<T> Items 
	{ 
		get => _items??new List<T>(); //items bilgisi yok ise boş bir liste döndür 
		set => _items = value; 
	}
}
