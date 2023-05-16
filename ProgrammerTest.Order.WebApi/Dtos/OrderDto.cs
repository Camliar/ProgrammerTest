namespace ProgrammerTest.Order.WebApi.Dtos
{
    public class OrderDto { 
        public long OrderNumber { get; set; }

        public string BuyerName { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string BillingZipCode { get; set; }

        public decimal OrderAmount { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
