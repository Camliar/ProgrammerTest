using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProgrammerTest.Order.WebApi.DbContexts;
using ProgrammerTest.Order.WebApi.Models;
using ProgrammerTest.Order.WebApi.Repository;
using System.Collections;

namespace ProgrammerTest.Order.Test;

[TestCaseOrderer("ProgrammerTest.Order.Test.PriorityOrderer", "XUnit.Coverlet.Collector")]
public class RepositoryTest
{
    private const string DBNAME = "testdb";
    public static DbContextOptions<OrderContext> CreateDbContextOptions(string databaseName)
    {
        var serviceProvider = new ServiceCollection().
            AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<OrderContext>();
        builder.UseInMemoryDatabase(databaseName)
            .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }


    [Theory, TestPriority(1)]
    [ClassData(typeof(DataForTest))]
    public async void InsertTest(OrderModel model, bool isSuccess)
    {
        var options = CreateDbContextOptions(DBNAME);
        var context = new OrderContext(options);

        var reposotory = new Repository<OrderModel>(context);


        var result = await reposotory.InsertAsync(model);

        Assert.Equal(isSuccess, result.Id > 0);
    }

    [Theory, TestPriority(2)]
    [InlineData(3)]
    public void GetListTest(int count)
    {
        var options = CreateDbContextOptions(DBNAME);
        var context = new OrderContext(options);

        var reposotory = new Repository<OrderModel>(context);


        var result = reposotory.GetAll().ToList();

        Assert.Equal(count, result.Count);
    }



}

public class DataForTest : IEnumerable<object[]>
{
    //这里是我们传递给Theory的数据
    private readonly List<object[]> _data = new List<object[]>
    {
        new object[] { new OrderModel() { BuyerName="周三" },true },
        new object[] { new OrderModel() { Id=2,BuyerName="王石", PurchaseOrderId = "TEST00001", OrderAmount = 1},true },
        new object[] { new OrderModel() { Id=3, BuyerName = "赵坎", PurchaseOrderId = "T0002"},true },
    };

    public IEnumerator<object[]> GetEnumerator()
    { return _data.GetEnumerator(); }

    IEnumerator IEnumerable.GetEnumerator()
    { return GetEnumerator(); }
}