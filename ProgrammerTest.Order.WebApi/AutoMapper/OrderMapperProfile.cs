using AutoMapper;

namespace ProgrammerTest.Order.WebApi.AutoMapper;

public class OrderMapperProfile : Profile
{
    public OrderMapperProfile()
    {
        CreateMap<OrderModel, OrderDto>()
            .ForMember(t => t.OrderNumber, opt => opt.MapFrom(src => src.Id))
            .ForMember(t => t.PurchaseOrderNumber, opt => opt.MapFrom(src => src.PurchaseOrderId));

        CreateMap<OrderDto, OrderModel>()
            .ForMember(t => t.Id, opt => opt.MapFrom(src => src.OrderNumber))
            .ForMember(t => t.PurchaseOrderId, opt => opt.MapFrom(src => src.PurchaseOrderNumber));

        CreateMap<OrderCreateDto, OrderModel>().ForMember(t => t.PurchaseOrderId, opt => opt.MapFrom(src => src.PurchaseOrderNumber));
    }
}
