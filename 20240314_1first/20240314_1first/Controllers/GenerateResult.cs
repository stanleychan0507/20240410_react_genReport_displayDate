using _20240314_1first.DTO;
using _20240314_1first.Model;

namespace _20240314_1first.Controllers
{
    public class GenerateResult
    {
        private readonly BikestoresContext _context;

        public GenerateResult(BikestoresContext context)
        {
            _context = context;
        }

        public ReportProductDTO GenerateProductReport(int customerId, string mode)
        {
            var orderItems = _context.Orders.Where(x => x.CustomerId == customerId).SelectMany(c => c.OrderItems);

            var totalPriceOfOrder = orderItems.Sum(x => x.ListPrice * x.Quantity);
            var totalCountofProducts = orderItems.Count(x => x.ProductId != null);
            var avgPriceForProduct = totalPriceOfOrder / totalCountofProducts;

            var result = new ReportProductDTO()
            {
                CustomerId = customerId,
                Mode = mode,
                TotalPrice = totalPriceOfOrder,
                TotalCountofProducts = totalCountofProducts,
                AvgPriceForProducts = avgPriceForProduct,
                ProductDetails = orderItems.Select(x => new ProductDetail()
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName,
                    ModelYear = x.Product.ModelYear,
                    ListPrice = x.ListPrice,
                    Quantity = x.Quantity
                })
                .ToList(),
            };
            return result;
        }
        public ReportBrandDTO GenerateBrandReport(int customerId, string mode)
        {
            var orderItems = _context.Orders.Where(x => x.CustomerId == customerId).SelectMany(c => c.OrderItems);

            var totalPriceOfOrder = orderItems.Sum(x => x.ListPrice * x.Quantity);
            var groupByBrand = orderItems.GroupBy(oi => oi.Product.BrandId);
            var totalCountofBrands = orderItems.Select(x => x.Product.Brand.BrandName).Distinct().Count();
            var avgPriceForBrands = totalPriceOfOrder / totalCountofBrands;

            var result = new ReportBrandDTO()
            {
                CustomerId = customerId,
                Mode = mode,
                TotalPrice = totalPriceOfOrder,
                TotalCountofBrands = totalCountofBrands,
                AvgPriceForBrands = avgPriceForBrands,
                BrandDetails = groupByBrand.Select(x => new BrandDetail()
                {
                    BrandId = x.Key,
                    BrandName = x.Select(oi => oi.Product.Brand.BrandName).First(),
                }).ToList(),
            };
            return result;
        }
        public ReportCategoryDTO GenerateCategoryReport(int customerId, string mode)
        {
            var orderItems = _context.Orders.Where(x => x.CustomerId == customerId).SelectMany(c => c.OrderItems);

            var totalPriceOfOrder = orderItems.Sum(x => x.ListPrice * x.Quantity);
            var groupByCategory = orderItems.GroupBy(oi => oi.Product.CategoryId);
            var totalCountofCategories = orderItems.Select(x => x.Product.CategoryId).Distinct().Count();
            var avgPriceForCategories = totalPriceOfOrder / totalCountofCategories;

            var result = new ReportCategoryDTO()
            {
                CustomerId = customerId,
                Mode = mode,
                TotalPrice = totalPriceOfOrder,
                TotalCountofCategories = totalCountofCategories,
                AvgPriceForCategories = avgPriceForCategories,
                CategoryDetails = groupByCategory.Select(x => new CategoryDetail()
                {
                    CategoryId = x.Key,
                    CategoryName = x.Select(oi => oi.Product.Category.CategoryName).First(),
                }).ToList(),
            };

            return result;
        }

    }
}