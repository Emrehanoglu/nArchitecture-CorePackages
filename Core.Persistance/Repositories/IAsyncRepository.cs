using Core.Persistance.Paging;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Repositories;

//hangi tip ile calısacağımı bu interface 'i cağırırken belirtilecek
//aynı zamanda id 'nin veri tipinide belirtmem gerekiyor, Guid mesela olabiliyordu.
//where kosulu ile belirtilen TEntityId -> TEntity 'nin id 'si olmalı diyorum,yani farklı bir id veri tipi yazamasın
public interface IAsyncRepository<TEntity,TEntityId> where TEntity : Entity<TEntityId> 
{
	Task<TEntity?> GetAsync(
		Expression<Func<TEntity,bool>> predicate, //get yaparken lambda ile datayı alabilirim
		Func<IQueryable<TEntity>,IIncludableQueryable<TEntity,object>>? include=null, //data çekerken join ile de çekebilirim, join destegi getirildi
		bool withDeleted=false, //veritabanında silinmiş olanları getirsin mi getirmesin mi şeklinde de dataları alabileceğim
		bool enableTracking=true, //entityframework 'un izleme desteği
		CancellationToken cancellationToken = default); //iptal etme işlemi için

	Task<Paginate<TEntity>> GetListAsync( //sayfalı listeleme yapısı
		Expression<Func<TEntity, bool>>? predicate = null, //get yaparken lambda ile dataları alabilirim
		Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, //orderby ile sıralama yapabilirim
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, //dataları çekerken join ile de çekebilirim, join destegi getirildi
		int index = 0, //sayfalı calısmak için, kacıncı sayfa bilgisi
		int size = 10, //sayfalı calısmak için, her sayfada kac adet olsun
		bool withDeleted = false, //veritabanında silinmiş olanları getirsin mi getirmesin mi şeklinde de dataları alabileceğim
		bool enableTracking = true, //entityframework 'un izleme desteği
		CancellationToken cancellationToken = default); //iptal etme işlemi için

	Task<Paginate<TEntity>> GetListByDynamicAsync( //
		DynamicQuery dynamic,
		Expression<Func<TEntity, bool>>? predicate = null,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
		int index = 0,
		int size = 10,
		bool withDeleted = false,
		bool enableTracking = true,
		CancellationToken cancellationToken = default
	);

	Task<bool> AnyAsync( //aradığım veri var mı yok mu bilgisi donecek
	   Expression<Func<TEntity, bool>>? predicate = null, //vereceğim koşulda data var mı yok mu bilgisi donecek
	   bool withDeleted = false,
	   bool enableTracking = true,
	   CancellationToken cancellationToken = default
   );

	Task<TEntity> AddAsync(TEntity entity); //apiden gelecek olan entity bilgilerini veritabanına kaydet
	
	Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities); //apiden gelecek olan çoklu entity bilgilerini veritabanına kaydet

	Task<TEntity> UpdateAsync(TEntity entity); //apiden gelecek olan entity bilgilerini veritabanında güncelle

	Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities); //apiden gelecek olan çoklu entity bilgilerini veritabanında güncelle

	Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false); //apiden gelecek olan entity bilgilerini veritabanından sil 
	//permanent ile kalıcı mı sileyim yoksa, soft delete ile silindiğini işaretleyeyim mi bilgisi alınacak
	Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false); //apiden gelecek olan çoklu entity bilgilerini veritabanından sil
}