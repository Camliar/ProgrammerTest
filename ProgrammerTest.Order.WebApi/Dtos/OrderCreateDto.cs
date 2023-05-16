namespace ProgrammerTest.Order.WebApi.Dtos
{
    public class OrderCreateDto
    {
        public string BuyerName { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string BillingZipCode { get; set; }

        public decimal OrderAmount { get; set; }
    }
}
