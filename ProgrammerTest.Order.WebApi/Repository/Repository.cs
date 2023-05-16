using Z.EntityFramework.Plus;

namespace ProgrammerTest.Order.WebApi.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseModel, new()
{
    private readonly DbContext _dbContext;

    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<int> DeleteAsync(long keyValue, CancellationToken cancellationToken = default)
    {
        var rows = 0;
        // 查询当前上下文中，有没有ID实体
        var entity = _dbContext.Set<TEntity>().Local.FirstOrDefault(t => t.Id == keyValue) ??
                     new TEntity { Id = keyValue };
        _dbContext.Remove(entity);

        try
        {
            rows = await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            rows = 0;
        }

        return rows;
    }

    public async Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var entityType = typeof(TEntity);

        var newExpression = Expression.New(entityType);
        var paramExpression = Expression.Parameter(entityType, "e");
        var binding = Expression.Bind(entityType.GetMember("IsDeleted")[0], Expression.Constant(true));
        var memberInitExpression = Expression.MemberInit(newExpression, binding);
        var lambdaExpression = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExpression);
        return await _dbContext.Set<TEntity>().Where(whereExpression).UpdateAsync(lambdaExpression, cancellationToken);
    }

    public async Task<TResult> FetchAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, bool ascending = false, bool writeDb = false, bool noTracking = true, CancellationToken cancellationToken = default)
    {
        var query = GetDbSet(writeDb, noTracking).Where(whereExpression);

        if (orderByExpression is null)
            return await query.OrderByDescending(t => t.Id).Select(selector).FirstOrDefaultAsync(cancellationToken);

        return ascending
            ? await query.OrderBy(orderByExpression).Select(selector).FirstOrDefaultAsync(cancellationToken)
            : await query.OrderByDescending(orderByExpression).Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> FindAsync(long keyValue, Expression<Func<TEntity, dynamic>> navigationPropertyPath = null, bool writeDb = false, bool noTracking = true, CancellationToken cancellationToken = default)
    {
        var query = GetDbSet(writeDb, noTracking).Where(t => t.Id == keyValue);
        if (navigationPropertyPath is not null)
            return await query.Include(navigationPropertyPath).FirstOrDefaultAsync(cancellationToken);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, dynamic>> navigationPropertyPath = null, Expression<Func<TEntity, object>> orderByExpression = null, bool ascending = false, bool writeDb = false, bool noTracking = true, CancellationToken cancellationToken = default)
    {
        var query = GetDbSet(writeDb, noTracking).Where(whereExpression);

        if (navigationPropertyPath is not null)
            query = query.Include(navigationPropertyPath);

        if (orderByExpression is null)
            return await query.OrderByDescending(t => t.Id).FirstOrDefaultAsync(cancellationToken);

        return ascending
            ? await query.OrderBy(orderByExpression).FirstOrDefaultAsync(cancellationToken)
            : await query.OrderByDescending(orderByExpression).FirstOrDefaultAsync(cancellationToken);
    }

    public IQueryable<TEntity> GetAll(bool writeDb = false, bool noTracking = true)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();
        if (noTracking)
            query = query.AsNoTracking();
        return query;
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
         await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<int> InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        // 获取实体状态
        var entry = _dbContext.Entry(entity);

        return entry.State switch
        {
            // 如果实体没有被跟踪，必须要指定需要更新的列
            EntityState.Detached => throw new ArgumentException("实体没有被跟踪，需要指定更新的列"),
            EntityState.Added or EntityState.Deleted => throw new ArgumentException(
                $"{nameof(entity)},实体状态为{nameof(entry.State)}"),
            _ => await UpdateInternalAsync(entity, cancellationToken)
        };
    }

    protected async Task<int> UpdateInternalAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions, CancellationToken cancellationToken = default)
    {

        if (updatingExpressions.IsNullOrEmpty())
            await UpdateAsync(entity, cancellationToken);

        var entry = _dbContext.Entry(entity);

        if (entry.State is EntityState.Added or EntityState.Deleted)
            throw new ArgumentException($"{nameof(entity)},实体状态为{nameof(entry.State)}");

        if (entry.State is EntityState.Unchanged)
            return await Task.FromResult(0);

        if (entry.State == EntityState.Modified)
        {
            var propNames = updatingExpressions.Select(t => t.GetMemberName()).ToArray();
            await entry.Properties.ForEachAsync(property =>
            {
                if (!propNames.Contains(property.Metadata.Name))
                    property.IsModified = false;
            });
        }

        if (entry.State == EntityState.Detached)
        {
            entry.State = EntityState.Unchanged;
            await updatingExpressions.ForEachAsync(expression => entry.Property(expression).IsModified = true);
        }

        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TEntity>> updatingExpression, CancellationToken cancellationToken = default)
    {

        return await UpdateRangeInternalAsync(whereExpression, updatingExpression, cancellationToken);
    }

    private async Task<int> UpdateRangeInternalAsync(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<TEntity, TEntity>> updatingExpression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().Where(whereExpression).UpdateAsync(updatingExpression, cancellationToken);
    }

    public async Task<int> UpdateRangeAsync(Dictionary<long, List<(string propertyName, dynamic propertyValue)>> propertyNameAndValues, CancellationToken cancellationToken = default)
    {
        var existsEntities = _dbContext.Set<TEntity>().Local.Where(t => propertyNameAndValues.ContainsKey(t.Id));

        foreach (var item in propertyNameAndValues)
        {
            var entity = existsEntities?.FirstOrDefault(t => t.Id == item.Key) ?? new TEntity { Id = item.Key };
            var entry = _dbContext.Entry(entity);

            if (entry.State is EntityState.Detached)
                entry.State = EntityState.Unchanged;

            if (entry.State != EntityState.Unchanged) return await _dbContext.SaveChangesAsync(cancellationToken);
            {
                var info = propertyNameAndValues.FirstOrDefault(t => t.Key == item.Key).Value;
                info.ForEach(x =>
                {
                    entry.Property(x.propertyName).CurrentValue = x.propertyValue;
                    entry.Property(x.propertyName).IsModified = true;
                });
            }
        }

        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> wherePredicate, bool writeDb = false, bool noTracking = true)
    {
        if (wherePredicate == null)
            return GetDbSet(writeDb, noTracking);
        return GetDbSet(writeDb, noTracking).Where(wherePredicate);
    }

    protected IQueryable<TEntity> GetDbSet(bool writeDb, bool noTracking)
    {
        if (noTracking)
            return _dbContext.Set<TEntity>().AsNoTracking();

        return _dbContext.Set<TEntity>();
    }
}