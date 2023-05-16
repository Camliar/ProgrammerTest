namespace ProgrammerTest.Order.WebApi.Models;

public class BaseModel
{
    public long Id { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }
}
