namespace ProgrammerTest.Order.WebApi.Models;

public class OrderModel : BaseModel
{

    public string BuyerName { get; set; }

    public string PurchaseOrderId { get; set; }

    public string BillingZipCode { get; set; }

    public decimal OrderAmount { get; set; }
}
