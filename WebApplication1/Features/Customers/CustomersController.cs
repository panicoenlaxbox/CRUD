using System.Web.Mvc;
using AutoMapper;
using WebApplication1.Features.Customers.ViewModels;
using WebApplication1.Models;

namespace WebApplication1.Features.Customers
{
    public class CustomersController : Controller
    {
        private readonly ICustomersService _customersService;

        public CustomersController(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        public ActionResult Index(int page = 1, string sort = "Id", string sortDir = "ASC")
        {
            var model = new IndexViewModel
            {
                Result = _customersService.GetAll(page, 10, string.Format("{0} {1}", sort, sortDir), null)
            };
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new CreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = Mapper.Map<CreateViewModel, CustomerDto>(model);
            _customersService.Add(customer);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var customer = _customersService.Find(id);
            var model = Mapper.Map<CustomerDto, EditViewModel>(customer);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var customer = Mapper.Map<EditViewModel, CustomerDto>(model);
            _customersService.Update(customer);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _customersService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}