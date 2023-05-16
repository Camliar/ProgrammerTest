using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ProgrammerTest.Order.WebApi.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ApiControllerBase
    {
        private readonly IOrderAppService _orderAppService;
        private readonly IMapper _mapper;

        public OrderController(IOrderAppService orderAppService, IMapper mapper)
        {
            _orderAppService = orderAppService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Get([FromRoute] long id)
        {
            var order = await _orderAppService.FindAsync(id);

            if (order == null)
                return NotFound();

            return _mapper.Map<OrderDto>(order);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<OrderDto>>> GetList([FromQuery] OrderCreateDto search)
        {
            var condition = ExpressionCreator
                .New<OrderModel>()
                .AndIf(!string.IsNullOrWhiteSpace(search?.BuyerName), t => t.BuyerName.Contains(search.BuyerName))
                .AndIf(!string.IsNullOrWhiteSpace(search?.PurchaseOrderNumber), t => t.PurchaseOrderId.Contains(search.PurchaseOrderNumber))
                .AndIf(!string.IsNullOrWhiteSpace(search?.BillingZipCode), t => t.BillingZipCode.Contains(search.BillingZipCode));

            var orders = await _orderAppService.GetList(condition);

            if (orders == null)
                return new List<OrderDto>();

            return _mapper.Map<List<OrderDto>>(orders);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto input)
        {
            if (input == null)
                return NotFound();

            if (input.BillingZipCode.IsNullOrWhiteSpace())
                return NotFound();


            var order = await _orderAppService.FindAsync(t => t.PurchaseOrderId == input.PurchaseOrderNumber);
            if(order != null)
                return NoContent();

            order = _mapper.Map<OrderModel>(input);

            var result = await _orderAppService.InsertAsync(order);

            return _mapper.Map<OrderDto>(result);
        }
    }
}
