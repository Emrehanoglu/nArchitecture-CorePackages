using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance.Repositories;

public class EfRepositoryBase<TEntity,TEntityId,TContext>:IAsyncRepository<TEntity,TEntityId>, IRepository<TEntity,TEntityId>
	where TEntity : Entity<TEntityId>
	where TContext : DbContext
{
	protected readonly TContext Context;

	public EfRepositoryBase(TContext context)
	{
		Context = context;
	}

	public async Task<TEntity> AddAsync(TEntity entity)
	{
		entity.CreatedDate = DateTime.UtcNow; //bölgesel saat dilimine gore zamanı verecek
		await Context.AddAsync(entity);
		await Context.SaveChangesAsync();
		return entity;
	}

	public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
	{
		foreach (TEntity entity in entities)
			entity.CreatedDate = DateTime.UtcNow;
		await Context.AddRangeAsync(entities);
		await Context.SaveChangesAsync();
		return entities;
	}

	public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
	{
		IQueryable<TEntity> queryable = Query();
		if (!enableTracking) //false gelmiş ise 
			queryable = queryable.AsNoTracking(); //tracking özelliğini kapatalım
		if (withDeleted) //silinmiş olarak işaretlenenler de gelsin 
			queryable = queryable.IgnoreQueryFilters();
		if (predicate != null) //bir şart var ise
			queryable = queryable.Where(predicate);
		return await queryable.AnyAsync(cancellationToken);
	}

	public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
	{
		//dikkat edilmesi gereken, delete işlemini permanent mi yapıcam softDeleted mi yapıcam
		await SetEntityAsDeletedAsync(entity,permanent); //burası yukarıdaki duruma karar verecek

		await Context.SaveChangesAsync();
		return entity;
	}

	public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
	{
		//dikkat edilmesi gereken, delete işlemini permanent mi yapıcam softDeleted mi yapıcam
		await SetEntityAsDeletedAsync(entities, permanent); //burası yukarıdaki duruma karar verecek

		await Context.SaveChangesAsync();
		return entities;
	}

	public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
	{
		IQueryable<TEntity> queryable = Query();
		if (!enableTracking) //tracking yok ise tracking özelliğini kapat
			queryable = queryable.AsNoTracking();
		if (include != null) //join yapılacak bir tablo var ise onu ekle
			queryable = include(queryable);
		if (withDeleted) //silinmiş işaretli olanlar gelsin mi 
			queryable = queryable.IgnoreQueryFilters();
		return await queryable.FirstOrDefaultAsync(predicate,cancellationToken);
	}

	public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
	{
		// join, order by, where clauses gibi yapıların hepsine destek verir
		IQueryable<TEntity> queryable = Query();
		if (!enableTracking) //tracking yok ise tracking özelliğini kapat
			queryable = queryable.AsNoTracking();
		if (include != null) //join yapılacak bir tablo var ise onu ekle
			queryable = include(queryable);
		if (withDeleted) //silinmiş işaretli olanlar gelsin mi 
			queryable = queryable.IgnoreQueryFilters();
		if (predicate != null) //where clauses varsa onlarıda getir
			queryable = queryable.Where(predicate);
		if (orderBy != null) //sıralama koşulu varsa sırala
			return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken); //sayfalama yapısına çevir
		return await queryable.ToPaginateAsync(index, size, cancellationToken); //veritabanından dataları getir
	}

	public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
	{
		IQueryable<TEntity> queryable = Query().ToDynamic(dynamic); //query 'i dynamic 'e cevir ve devamında yukarıdakilerin aynısnı yap
		if (!enableTracking)
			queryable = queryable.AsNoTracking();
		if (include != null)
			queryable = include(queryable);
		if (withDeleted)
			queryable = queryable.IgnoreQueryFilters();
		if (predicate != null)
			queryable = queryable.Where(predicate);
		return await queryable.ToPaginateAsync(index, size, cancellationToken);
	}

	public IQueryable<TEntity> Query()
	{
		throw new NotImplementedException();
	}

	public async Task<TEntity> UpdateAsync(TEntity entity)
	{
		entity.UpdatedDate = DateTime.UtcNow; //bölgesel saat dilimine gore zamanı verecek
		Context.Update(entity);
		await Context.SaveChangesAsync();
		return entity;
	}

	public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
	{
		foreach (TEntity entity in entities)
			entity.UpdatedDate = DateTime.UtcNow; //bölgesel saat dilimine gore zamanı verecek
		Context.Update(entities);
		await Context.SaveChangesAsync();
		return entities;
	}

	//DeleteRangeAsync için
	private async Task SetEntityAsDeletedAsync(ICollection<TEntity> entities, bool permanent)
	{
		foreach (TEntity entity in entities)
			await SetEntityAsDeletedAsync(entity,permanent);
	}

	//DeleteAsync için
	protected async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
	{
		if (!permanent)
		{
			CheckHasEntityHaveOneToOneRelation(entity); //bire bir ilişkisi varmı ona bakacak, olmaması gerekiyor yanı false donmesi lazım
			await setEntityAsSoftDeletedAsync(entity); // entity 'nin deletedDate 'i var mı yok mu bakacak,
													   // var ise softDelete 'diri
													   // yok ise ilgili entity 'i tüm ilişkileri ile beraber softDelete yapacak
		}
		else
		{
			Context.Remove(entity);
		}
	}

	protected void CheckHasEntityHaveOneToOneRelation(TEntity entity)
	{
		bool hasEntityHaveOneToOneRelation =
			Context
				.Entry(entity)
				.Metadata.GetForeignKeys()
				.All(
					x =>
						x.DependentToPrincipal?.IsCollection == true
						|| x.PrincipalToDependent?.IsCollection == true
						|| x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
				) == false;
		if (hasEntityHaveOneToOneRelation)
			throw new InvalidOperationException(
				"Entity has one-to-one relationship. Soft Delete causes problems if you try to create entry again by same foreign key."
			);
	}

	private async Task setEntityAsSoftDeletedAsync(IEntityTimestamps entity)
	{
		if (entity.DeletedDate.HasValue)
			return;
		entity.DeletedDate = DateTime.UtcNow;

		var navigations = Context
			.Entry(entity)
			.Metadata.GetNavigations()
			.Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
			.ToList();
		foreach (INavigation? navigation in navigations)
		{
			if (navigation.TargetEntityType.IsOwned())
				continue;
			if (navigation.PropertyInfo == null)
				continue;

			object? navValue = navigation.PropertyInfo.GetValue(entity);
			if (navigation.IsCollection)
			{
				if (navValue == null)
				{
					IQueryable query = Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
					navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();
					if (navValue == null)
						continue;
				}

				foreach (IEntityTimestamps navValueItem in (IEnumerable)navValue)
					await setEntityAsSoftDeletedAsync(navValueItem);
			}
			else
			{
				if (navValue == null)
				{
					IQueryable query = Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
					navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
						.FirstOrDefaultAsync();
					if (navValue == null)
						continue;
				}

				await setEntityAsSoftDeletedAsync((IEntityTimestamps)navValue);
			}
		}

		Context.Update(entity);
	}

	protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
	{
		Type queryProviderType = query.Provider.GetType();
		MethodInfo createQueryMethod =
			queryProviderType
				.GetMethods()
				.First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
				?.MakeGenericMethod(navigationPropertyType)
			?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
		var queryProviderQuery =
			(IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression })!;
		return queryProviderQuery.Where(x => !((IEntityTimestamps)x).DeletedDate.HasValue);
	}

	public TEntity? Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true)
	{
		throw new NotImplementedException();
	}

	public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
	{
		throw new NotImplementedException();
	}

	public Paginate<TEntity> GetListByDynamic(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
	{
		throw new NotImplementedException();
	}

	public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true)
	{
		throw new NotImplementedException();
	}

	public TEntity Add(TEntity entity)
	{
		throw new NotImplementedException();
	}

	public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
	{
		throw new NotImplementedException();
	}

	public TEntity Update(TEntity entity)
	{
		throw new NotImplementedException();
	}

	public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
	{
		throw new NotImplementedException();
	}

	public TEntity Delete(TEntity entity, bool permanent = false)
	{
		throw new NotImplementedException();
	}

	public ICollection<TEntity> DeleteRange(ICollection<TEntity> entity, bool permanent = false)
	{
		throw new NotImplementedException();
	}
}
