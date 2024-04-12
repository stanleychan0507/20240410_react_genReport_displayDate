using _20240314_1first.Model;
using Microsoft.EntityFrameworkCore;
using static _20240314_1first.Controllers.BikeStoresController;

namespace _20240314_1first.Controllers
{
    public class Learning
    {
        private readonly BikestoresContext _context;
        public Learning(BikestoresContext context) { _context = context; }

        public async Task LearnGroupBy()
        {
            var productGroupByBrandId = await _context.Product.GroupBy(product => product.BrandId)
                   .Select(group => new TestDto
                   {
                       BrandId = group.Key,
                       ProductNames = group.Select(group => group.ProductName).ToList()
                   }).ToListAsync();
            foreach (var product in productGroupByBrandId)
            {
                Console.WriteLine("BrandID: {0}, ProductNames : {1}", product.BrandId, string.Join(",", product.ProductNames));
            }
            // {BrandId, Product}
            // count: 122, {1, [Electra Townie Original 21D - 2016, Electra Cruiser 1 (24-Inch) - 2016, Electra Girl's Hawaii 1 (16-inch) - 2015/2016 ......] }
            // count: 11, {2, [Ferrari, Haro Shift R3 - 2017]}
            // count: 2, [{3, asd} ....]
            // count: 3, [{4, asd} ....]
            // count: 1, [{5, asd} ....]
            // count: 3, [{6, asd} ....]
            // count: 23, [{7, asd} ....]
            // count: 25, [{8, asd} ....]
            // count: 134,[{9, asd} ....]
            // count: 1, [{13, sad}, ........]
        }

        public async void weatherAPI()
        {
            //var response = await (await "https://new-api.smg.gov.mo/day"
            //    .WithHeaders(new
            //    {
            //        User_Agent = "yes"
            //    })
            //    .PostJsonAsync(new { date = "today" }))
            //    .GetJsonAsync<ReasponseA>();
        }


    }
    public class TestDto
    {
        public int BrandId { get; set; }
        public List<string> ProductNames { get; set; }

    }

}
