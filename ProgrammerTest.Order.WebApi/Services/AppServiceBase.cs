using ProgrammerTest.Order.WebApi.Common.Utils;
using ProgrammerTest.Order.WebApi.Repository;

namespace ProgrammerTest.Order.WebApi.Services;

public class AppServiceBase<TEntity> : IAppService<TEntity> where TEntity : BaseModel, new()
{
    private IRepository<TEntity> _repository;

    public AppServiceBase(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {

        entity.Id = IdGenerator.NextId();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = default;
        var result = await _repository.InsertAsync(entity);
        return result;
    }

    public async Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>>[] updatingExpressions)
    {
        entity.UpdateTime = DateTime.Now;
        var result = await _repository.UpdateAsync(entity, updatingExpressions);
        return result;
    }
    public async Task<int> DeleteAsync(long id)
    {
        var result = await _repository.DeleteAsync(id);
        return result;
    }

    public async Task<TEntity> FindAsync(long id)
    {
        return await _repository.FindAsync(id);
    }

    public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await _repository.FindAsync(condition);
    }

    public async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> condition = null)
    {
        return await _repository.Where(condition).ToListAsync();
    }
}
