namespace ProgrammerTest.Order.WebApi.Services;

public class OrderAppService : AppServiceBase<OrderModel>, IOrderAppService
{
    public OrderAppService(IRepository<OrderModel> repository) : base(repository)
    {
    }
}

