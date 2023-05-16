using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProgrammerTest.Order.WebApi.DbContexts;
using ProgrammerTest.Order.WebApi.Models;
using ProgrammerTest.Order.WebApi.Repository;
using System.Collections;

namespace ProgrammerTest.Order.Test;

[TestCaseOrderer("ProgrammerTest.Order.Test.PriorityOrderer", "ProgrammerTest.Order.Test")]
public class RepositoryTest
{
    private const string DBNAME = "testdb";
    private static OrderContext _orderDbContext;

    private static void CreateDbContext()
    {
        if (_orderDbContext is not null)
            return;

        var serviceProvider = new ServiceCollection().
            AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<OrderContext>();
        builder.UseInMemoryDatabase(DBNAME)
            .UseInternalServiceProvider(serviceProvider);

        var options = builder.Options;
        _orderDbContext = new OrderContext(options);
    }


    [Theory]
    [TestPriority(0)]
    [ClassData(typeof(DataForTest))]
    public async void InsertTest(OrderModel model, bool isSuccess)
    {
        CreateDbContext();

        var reposotory = new Repository<OrderModel>(_orderDbContext);


        var result = await reposotory.InsertAsync(model);

        Assert.Equal(isSuccess, result.Id > 0);
    }


    [Theory]
    [TestPriority(1)]
    [InlineData(3)]
    public void GetListTest(int count)
    {
        CreateDbContext();

        var reposotory = new Repository<OrderModel>(_orderDbContext);


        var result = reposotory.GetAll().ToList();

        Assert.Equal(count, result.Count);
    }



}

public class DataForTest : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        new object[] { new OrderModel() { BuyerName="周三" },true },
        new object[] { new OrderModel() { Id=2,BuyerName="王石", PurchaseOrderId = "TEST00001", OrderAmount = 1},true },
        new object[] { new OrderModel() { Id=3, BuyerName = "赵坎", PurchaseOrderId = "T0002"},true },
    };

    public IEnumerator<object[]> GetEnumerator()
    { 
        return _data.GetEnumerator(); 
    }

    IEnumerator IEnumerable.GetEnumerator()
    { 
        return GetEnumerator(); 
    }
}