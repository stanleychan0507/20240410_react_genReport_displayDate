using _20240314_1first.DTO;
using _20240314_1first.Model;
using Castle.Core.Resource;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using static _20240314_1first.Controllers.BikeStoresController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _20240314_1first.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeStoresController : ControllerBase
    {
        private readonly BikestoresContext _context;
        private readonly Learning _learning;
        private readonly GenerateResult _generateResult;

        public BikeStoresController(BikestoresContext c, Learning learning, GenerateResult generateResult)
        {
            _context = c;
            _learning = learning;
            _generateResult = generateResult;
        }

        //[HttpGet]
        //[Route("[action]/Join2Table")]
        //public async Task<ActionResult<List<Join2TableDTO>>> Join2Table()
        //{
        //    // Long form SQL query
        //    //var query = from p in _context.Product
        //    //            join b in _context.Brands on p.BrandId equals b.BrandId
        //    //            orderby b.BrandName descending
        //    //            select new ProductDTO
        //    //            {
        //    //                BrandName = b.BrandName,
        //    //                ProductName = p.ProductName,
        //    //                ListPrice = p.ListPrice,
        //    //                ModelYear = p.ModelYear
        //    //            };

        //    return _context.Product.Where(x => x.ProductId > 0)
        //    .Join(_context.Brands, p => p.BrandId, b => b.BrandId,
        //    (p, b) => new Join2TableDTO
        //    {
        //        BrandName = b.BrandName,
        //        ProductName = p.ProductName,
        //        ListPrice = p.ListPrice,
        //        ModelYear = p.ModelYear,
        //    }
        //    ).OrderByDescending(x => x.BrandName).ToList();
        //}

        [HttpGet]
        [Route("Join3Table")]
        public List<Join2TableDTO> Join3Table()
        {
            var query2 = from p in _context.Product
                         join b in _context.Brands on p.BrandId equals b.BrandId
                         join c in _context.Categories on p.CategoryId equals c.CategoryId
                         select new Join2TableDTO
                         {
                             BrandName = p.Brand.BrandName,
                             CategoryName = p.Category.CategoryName,
                             ProductName = p.ProductName,
                             ListPrice = p.ListPrice,
                             ModelYear = p.ModelYear,
                         };

            return query2.ToList();
        }

        [HttpGet]
        [Route("[action]/ProductBrand")]
        public async Task<ActionResult<List<Join2TableDTO>>> GetProductByBrand(int id)
        {
            var query3 = _context.Brands.Where(x => x.BrandId == id)
                .Join(_context.Product, b => b.BrandId, p => p.BrandId, (b, p) => new Join2TableDTO()
                {
                    BrandName = b.BrandName,
                    ProductName = p.ProductName,
                    ListPrice = p.ListPrice,
                    ModelYear = p.ModelYear,
                });

            return await query3.ToListAsync();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<List<Join3TableDTO>>> UpdateProductName(int id, [FromBody] Join3TableDTO productMap)
        {
            var product = _context.Product.FirstOrDefault(x => x.ProductId == id)
                ?? throw new ArgumentNullException($"The Product ID : {id} can not be found.");
            product.ProductName = productMap.ProductName;
            _context.Update(product);
            await _context.SaveChangesAsync();

            return Ok(Result.SwitchCaseToSuccess("PUT"));
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = _context.Product.FirstOrDefault(x => x.ProductId == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(Result.SwitchCaseToSuccess("DELETE"));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateBrand([FromBody] CreateBrandDTO brandFromBody)
        {
            Console.WriteLine("here");
            if (_context.Brands.FirstOrDefault(x => x.BrandName == brandFromBody.BrandName) != null) { return BadRequest(); }

            Brand brand = new Brand { BrandName = brandFromBody.BrandName };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return Ok(Result.SwitchCaseToSuccess("POST"));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductDTO productFromBody)
        {
            var mapper = new ProductDtoMapper();
            var product = mapper.ToProduct(productFromBody);

            // check if CategoryId exist, otherwise fail
            if ((_context.Categories.FirstOrDefault(x => x.CategoryId == productFromBody.CategoryId)) != null)
            {
                // check if BrandId exist, otherwise fail
                if ((_context.Brands.FirstOrDefault(x => x.BrandId == productFromBody.BrandId)) != null)
                {
                    _context.Product.Add(product);
                    await _context.SaveChangesAsync();
                    return Ok(Result.SwitchCaseToSuccess("POST"));
                }
                else
                {
                    return Ok(Result.ToFailed("POST"));
                }
            }
            else
            {
                return Ok(Result.ToFailed("POST"));
            }
        }


        // Access staff db by staff email, 
        // Then Start an order by asking customer id
        // If customer id exist, then return Ok()
        // If customer id does not exist, then register new customer to db
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> StaffOrderController()
        {
            Console.Clear();
            Console.WriteLine("Hello staff, type your email.\r");
            var input_staff_email = Console.ReadLine();
            var staff_email = _context.Staffs.FirstOrDefault(x => x.Email == input_staff_email);
            Console.WriteLine("------------------------");

            if (staff_email == null)
            {
                return BadRequest($"{staff_email} does not exist");
            }

            Console.WriteLine("Create your order by typing Customer id.\r");

            int tryparse_input_cust_id;
            int.TryParse(Console.ReadLine(), out tryparse_input_cust_id);
            if (tryparse_input_cust_id == 0) { return BadRequest($"{tryparse_input_cust_id} Cannot be zero, or incorrect input detected"); }
            var cust_id = _context.Customers.FirstOrDefault(x => x.CustomerId == tryparse_input_cust_id);
            Console.WriteLine("------------------------");

            if (cust_id == null)
            {
                Console.WriteLine("Type your first name : \r");
                string firstName = Console.ReadLine();
                Console.WriteLine("Type your last name : \r");
                string lastName = Console.ReadLine();
                Console.WriteLine("------------------------");

                Console.WriteLine("Create customer");
                var cust = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = firstName + "." + lastName + "@gmail.com"
                };
                _context.Customers.Add(cust);
                await _context.SaveChangesAsync();
                var customer = _context.Customers.FirstOrDefault(x => x.Email == firstName + "." + lastName + "@gmail.com");
                tryparse_input_cust_id = customer.CustomerId;
            }
            return Ok();
        }

        [HttpGet ("AddOrderItemApp")]
        public async Task<ActionResult> AddOrderItemApp()
        {
            Console.Clear();
            Console.WriteLine("Enter an integer, how many things do you want to buy?");
            int numOfProduct = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"What product do you want to buy?");
            List<string> list = new List<string>();
            for (int i = 0; i < numOfProduct; i++) { list.Add(Console.ReadLine()); }

            var order = new Order()
            {
                StaffId = 1,
                Staff = _context.Staffs.First(x => x.StaffId == 1),
                StoreId = 1,
                Store = _context.Stores.First(x => x.StoreId == 1)
            };
            //list.Where()

            var orderItem = list.Select(name =>
            _context.Product.FirstOrDefault(p => p.ProductName == name))
                .GroupBy(p => p.ProductId).Select((group, index) =>
                new OrderItem()
                {
                    OrderId = order.OrderId,
                    Quantity = group.Count(),
                    Product = group.First(),
                    Order = order,
                    ItemId = index,
                    ProductId = group.First().ProductId
                }); ;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _context.OrderItems.AddRange(orderItem);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //New requirement : 
        //group by product name, brand, category
        //Total price, total orderid, avg(totalprice/numoforder)
        [HttpGet]
        public async Task<ActionResult> Report(int customerId, string mode)
        {
            // check if customer ID exist 
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (customer is null) return BadRequest("The customer does not exist");

            IReportDTO result = mode switch
            {
                "product" => _generateResult.GenerateProductReport(customerId, mode),

                "brand" => _generateResult.GenerateBrandReport(customerId, mode),

                "category" => _generateResult.GenerateCategoryReport(customerId, mode),

                _ => throw new NoNullAllowedException(),

            }; 

            return Ok(result);

        }
    }





    //Name = group.FirstOrDefault().Product.ProductName,
    //TotalPrice = group.Sum(x => x.ListPrice* x.Quantity),
    //TotalCountofProduct = Convert.ToInt32(group.Count(x => x.OrderId != null)),
    //AvgPrice = Convert.ToInt32(group.Sum(x => x.ListPrice) / group.Count(x => x.OrderId != null))

   



}