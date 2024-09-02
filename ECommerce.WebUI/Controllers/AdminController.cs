using ECommerce.Business.Abstract;
using ECommerce.DataAccess.Abstraction;
using ECommerce.Entities.Models;
using ECommerce.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: ProductController
        public async Task<ActionResult> Index(int page = 1, int category = 0)
        {
            int pageSize = 10;
            var items = await _productService.GetAllByCategoryAsync(category);

            var model = new ProductListViewModel
            {
                Products = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentCategory = category,
                PageCount = (int)Math.Ceiling(items.Count / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = page
            };
            return View(model);
        }

        public async Task<IActionResult> RemoveProduct(int id,int page,int category) 
        {
            await _productService.DeleteAsync(id);
            return RedirectToAction("Index","Admin",new {page = page,category = category});
        }
        public async Task<IActionResult> RemoveCategory(int id)
        {
            await _categoryService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddProduct(int page, int category) 
        {
            TempData["page"] = page;
            TempData["category"] = category;
            var vm = new AdminAddProductViewModel()
            {
                Product = new Product() { }
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AdminAddProductViewModel vm,int page, int category) 
        {
            var count = _productService.GetAllAsync().Result.Count;
            if (ModelState.IsValid) 
            {
                Product product = new Product()
                {
                    ProductName = vm.Product.ProductName,
                    SupplierId = vm.Product.SupplierId,
                    CategoryId = vm.Product.CategoryId,
                    QuantityPerUnit = vm.Product.QuantityPerUnit,
                    UnitPrice = vm.Product.UnitPrice,
                    UnitsInStock = vm.Product.UnitsInStock,
                    UnitsOnOrder = vm.Product.UnitsOnOrder,
                    ReorderLevel = vm.Product.ReorderLevel,
                    Discontinued = vm.Product.Discontinued,
                };
                await _productService.AddAsync(product);
                return RedirectToAction("Index", "Admin", new { page = TempData["page"], category = TempData["category"] });
            }
            return View();
        }

        [HttpGet]
        public IActionResult AddCategory() 
        {
            var vm = new AdminAddCategoryViewModel()
            {
                Category = new Category() { }
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AdminAddCategoryViewModel vm) 
        {
            if (ModelState.IsValid) 
            {
                var category = new Category()
                {
                    CategoryName = vm.Category.CategoryName,
                    Description = vm.Category.Description,
                    Picture = vm.Category.Picture
                };
                await _categoryService.AddAsync(category);
                return RedirectToAction("Index");
            
            }
            return View();
        }

    }
}
