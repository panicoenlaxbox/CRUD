using AutoMapper;
using WebApplication1.Features.Customers;
using WebApplication1.Features.Customers.ViewModels;
using WebApplication1.Models;

namespace WebApplication1.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(configuration =>
            {
                configuration.AllowNullDestinationValues = true;
                Mapper.CreateMap<Customer, CustomerDto>(); //sale del servicio
                Mapper.CreateMap<CustomerDto, Customer>(); //entra al servicio
                Mapper.CreateMap<CreateViewModel, CustomerDto>(); //entra en el controlador
                Mapper.CreateMap<EditViewModel, CustomerDto>(); //entra en el controlador
                Mapper.CreateMap<CustomerDto, EditViewModel>(); //entra en la vista
            });
        }
    }
}