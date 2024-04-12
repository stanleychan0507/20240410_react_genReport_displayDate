using _20240314_1first.Model;

namespace _20240314_1first.Controllers
{
    public class Method
    {
        public static Order createOrder (int id)
        {
            var order = new Order
            {
                CustomerId = id,
                StoreId = 1,
                StaffId = 1,
            };

            return order;
        }
    }
}
