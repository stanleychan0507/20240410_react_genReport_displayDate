using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestBikeStore.SqlModels;

namespace TestBikeStore.Controllers;

#region you should put these class to another file

public enum ReportMode
{
    Brand    = 0,
    Product  = 1,
    Category = 2,
    All      = 3
}

public class Report
{
    public ReportMode                Mode          { get; set; }
    public string                    CustomerName  { get; set; }
    public IEnumerable<ReportDetail> ReportDetails { get; set; }
}

public class ReportDetail
{
    public string Item       { get; set; }
    public double TotalPrice { get; set; }
    public double AvgPrice   { get; set; }
    public int    ItemCount  { get; set; }
}

#endregion

[Route("api/[controller]")]
[ApiController]
public class MainController : Controller
{
    //this one doesn't mean DI , means global usages 
    private readonly BikeStoreContext _context;

    //DI happen in ctor we called constructor injection 
    /*MainController(BikeStoreContext context)
     *  name of the class (  params the DI from )
     */
    public MainController(BikeStoreContext context)
    {
        _context = context; //so if here is null possbily you  didn't put the context to container , check program.cs
    }

    [HttpGet("/GetLatestOrder")] // if i dont specific anything , than the route will be local/api/Main/GetLastestOrder
    public async Task<ActionResult> GetLatestOrder()
    {
        //First or default possblily null , you should chceck it ,before put it to DTO 
        return Ok(await _context.Orders.Include(x => x.OrderItems).ThenInclude(y => y.Product).FirstOrDefaultAsync());
    }

    /// <summary>
    ///     In this example ,to make it ,we assume the store id is 1
    ///     first we have the names of products , than we select it
    ///     please enchances this according to the difficultiy
    /// </summary>
    /// <param name="products"></param>
    /// <returns></returns>
    [HttpPost("CreateOrderByProductNames")]
    public async Task<ActionResult<Order>> CreateOrderByProductNames([FromBody] List<string> products)
    {
        #region please increase difficulity by removing assumeption

        var storeId = 1; // store_id	store_name	phone	email	street	city	state	zip_code
        // 1	Santa Cruz Bikes	(831) 476-4321	santacruz@bikes.shop	3700 Portola Drive	Santa Cruz	CA	95060
        var customerId = 1; //customer_id	first_name	last_name	phone	email	street	city	state	zip_code
        // 1	Debra	Burks	NULL	debra.burks@yahoo.com	9273 Thorne Ave. 	Orchard Park	NY	14127

        #endregion

        #region Identity when is IEnumerable , IQueryable

        // if should always null checking when using FirstOrDefault , please google it 
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        var staff    = await _context.Staffs.FirstOrDefaultAsync(s => s.StoreId       == storeId);
        var store    = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId       == storeId);
        // var stocks             = await _context.Stocks.FirstOrDefaultAsync(s => s.StoreId == storeId);
        var inventoryLeftProducts = await (from product in _context.Products
                                           join stock in _context.Stocks on product.ProductId equals stock.ProductId
                                           where stock.StoreId  == storeId && products.Contains(product.ProductName) &&
                                                 stock.Quantity > 0
                                           select product).ToListAsync();

        #endregion

        #region Linq practice show cases

        //check if all products are enough in stock
        if (inventoryLeftProducts.Count != products.Count)
        {
            //ToList is because if it is IEnumerable than it run mutliple times
            var productNames = inventoryLeftProducts.Select(c => c.ProductName);
            //.ToList();
            var foundProduct    = string.Join("\n", productNames);
            var notFoundProduct = products.Except(productNames);
            return BadRequest($"found in inventory {foundProduct} \n Not Found in Inventory : {notFoundProduct} ");
        }

        #endregion

        #region create Order

        Order order = new()
        {
            OrderStatus  = 0,                                   //assumeption
            OrderDate    = DateOnly.FromDateTime(DateTime.Now), //assumeption
            RequiredDate = DateOnly.FromDateTime(DateTime.Now), //assumeption
            ShippedDate  = DateOnly.FromDateTime(DateTime.Now), //assumeption
            Customer     = customer,
            OrderItems = inventoryLeftProducts.Select((p, index) => new OrderItem
            {
                ItemId    = index,
                Quantity  = 1,           //assumpetion
                ListPrice = p.ListPrice, //you should *count if more than 1 
                Discount  = 0,           //assumeption
                Product   = p
            }).ToList(),
            Staff = staff,
            Store = store
        };

        #endregion

        await _context.Orders.AddAsync(order);
        var saveResult =
            await _context
               .SaveChangesAsync(); //remember you always have to saveChange if update insert delete (select no need)
        if (saveResult == 0) return BadRequest();
        return Ok(order.OrderId);
    }

    [HttpPost("[action]/{customerId}/{mode}")]
    public async Task<ActionResult> GenenerateReport(int customerId, ReportMode mode)
    {
        //at first we should check the customer is existed

        #region if you wanted check it only

        //var isCustomerExisted = await _context.Customers.AnyAsync(x => x.CustomerId == customerId);
        //if (isCustomerExisted ) return BadRequest("fk you customer not existed");

        #endregion

        #region but since we wanted the customer name is included in the report

        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        if (customer is null) return BadRequest("fk you customer not existed");

        #endregion

        #region flatten the order items , we only looking for OrderStatus is completed

        var orderItems = _context.Orders
                                  //later requirment i would change this to group by order status details
                                 .Where(x => x.CustomerId == customerId && x.OrderStatus == OrderStatus.Completed)
                                 .SelectMany(c => c.OrderItems);

        #endregion

        #region IEnumerable Vs IQueryable ,  Func<T> Vs Expression<T> you should always google it why

        /*now , i did the tricks here , since i dont wanted the linq be so large
          so what i did is , left the expressionSelector https://stackoverflow.com/questions/61412241/build-expression-to-filter-data-ef-core
           guess why i can put function inside IQueryable
         */
        var reportDetails = mode switch
                            {
                                ReportMode.All =>
                                    throw new NotImplementedException(), // i left this to you guys think of
                                ReportMode.Brand   => GenerateReport(orderItems, oi => oi.Product.Brand.BrandName),
                                ReportMode.Product => GenerateReport(orderItems, oi => oi.Product.ProductName),
                                ReportMode.Category => GenerateReport(orderItems,
                                                                      oi => oi.Product.Category.CategoryName),
                                _ => throw new NotImplementedException() //yet we should have mode
                            };

        #endregion

        return Ok(new Report
        {
            CustomerName  = $"{customer.FirstName} - {customer.LastName}",
            ReportDetails = reportDetails,
            Mode          = mode
        });
    }

    /// <summary>
    ///     Genereate report by order items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="orders"> here we but the faltten orderItem</param>
    /// <param name="itemSelector">hahaha , my cutie tricks </param>
    /// <returns></returns>
    private IQueryable<ReportDetail> GenerateReport<T>(
        IQueryable<OrderItem> ordersItems,
        Expression<Func<OrderItem, T>> itemSelector) //so here if you put Func<> you maybe fucked in excepttion since it is IQueryable
    {
    
        return ordersItems
              .GroupBy(itemSelector)
              .Select(group => new ReportDetail
               {
                   Item = group.Key.ToString(), // hahaha , as long as i projected the "Key" correctly , it should be ok
                   TotalPrice = (double)group.Sum(oi => oi.ListPrice * oi.Quantity * (1 - oi.Discount)),
                   AvgPrice = (double)group.Average(oi => oi.ListPrice * oi.Quantity * (1 - oi.Discount)),
                   ItemCount = group.Sum(oi => oi.Quantity)
               });
    }
}