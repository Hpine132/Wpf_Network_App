using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Logics
{
    public class CategoryService
    {
        public List<Category> GetCategories()
        {
            using (var context = new NorthwindContext())
            {
                return context.Categories.ToList();
            }
        }

    }

}
