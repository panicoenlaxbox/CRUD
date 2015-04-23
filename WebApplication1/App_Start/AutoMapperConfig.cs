using AutoMapper;
using WebApplication1.Models;
using WebApplication1.ViewModels.Customers;

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
                Mapper.CreateMap<CustomersCreateViewModel, CustomerDto>(); //entra en el controlador
                Mapper.CreateMap<CustomersEditViewModel, CustomerDto>(); //entra en el controlador
                Mapper.CreateMap<CustomerDto, CustomersEditViewModel>(); //entra en la vista
            });
        }
    }
}