namespace ProgrammerTest.Order.WebApi.Services;

public interface IAppService<TEntity> where TEntity : BaseModel, new()
{
    Task<TEntity> InsertAsync(TEntity entity);

    Task<TEntity> FindAsync(long id);

    Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> condition);

    Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> condition = null);

    Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions);

    Task<int> DeleteAsync(long Id);
}