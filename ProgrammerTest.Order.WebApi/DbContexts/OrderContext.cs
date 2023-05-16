namespace ProgrammerTest.Order.WebApi.DbContexts;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {

    }

    public DbSet<OrderModel> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderModel>().HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);
    }
}
