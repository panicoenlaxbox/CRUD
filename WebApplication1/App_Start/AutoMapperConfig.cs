using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                Mapper.CreateMap<Customer, CustomerDto>();
                Mapper.CreateMap<CustomersEditViewModel, Customer>();
                Mapper.CreateMap<Customer, CustomersEditViewModel>();
                Mapper.CreateMap<CustomersCreateViewModel, Customer>();
            });
        }
    }
}