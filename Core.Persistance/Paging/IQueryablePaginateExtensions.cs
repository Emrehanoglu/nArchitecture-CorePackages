using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Paging;

public static class IQueryablePaginateExtensions
{
	public static async Task<Paginate<T>> ToPaginateAsync<T>( //bu metodu cagırdığımda alacağı parametreler
			this IQueryable<T> source, //listeleyeceğim data
			int index, //hangi sayfa
			int size, //sayfadaki data sayısı
			CancellationToken cancellationToken = default
		)
	{
		//count -> paginate tarafında olusturduğum bir property 'di.
		//ConfigureAwait(false) : await konfigürasyonu yapmak istemiyorum.
		int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

		//dataları listeli olarak, index*size elemanından itibaren, tek bir sayfada listeleyeceğim size kadar alıyorum
		//ConfigureAwait(false) : await konfigürasyonu yapmak istemiyorum.
		List<T> items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);

		//paginate yapısını artık geri dondurebilirim
		Paginate<T> list = new Paginate<T>
		{
			Count = count,
			Index = index,
			Items = items,
			Size = size,
			Pages = (int)Math.Ceiling(count / (double)size)
		};
		return list;
	}

	public static Paginate<T> ToPaginate<T>( //bu metodu cagırdığımda alacağı parametreler
			this IQueryable<T> source, //listeleyeceğim data
			int index, //hangi sayfa
			int size, //sayfadaki data sayısı
			CancellationToken cancellationToken = default
		)
	{
		//count -> paginate tarafında olusturduğum bir property 'di.
		int count = source.Count();

		//dataları listeli olarak, index*size elemanından itibaren, tek bir sayfada listeleyeceğim size kadar alıyorum
		var items = source.Skip(index * size).Take(size).ToList();

		//paginate yapısını artık geri dondurebilirim
		Paginate<T> list = new Paginate<T>
		{
			Count = count,
			Index = index,
			Items = items,
			Size = size,
			Pages = (int)Math.Ceiling(count / (double)size)
		};
		return list;
	}
}