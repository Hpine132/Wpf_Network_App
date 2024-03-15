using DataAccess.Models;
using System.Linq;

namespace DataAccess.Logics
{
    public class OrderDetailsService
    {
        public bool CheckOrderDetails(int productId)
        {
            using (var context = new NorthwindContext())
            {
                var orderDetails = context.OrderDetails.FirstOrDefault(o => o.ProductId == productId);
                if (orderDetails != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
