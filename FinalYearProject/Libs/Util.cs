using FinalYearProject.Data;
using FinalYearProject.ViewModels;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FinalYearProject.Libs
{
    public static class Util
    {



        public static string GetPrice(decimal price)
        {
            return "£" + price.ToString("0.00");
        }
        public static string GetPrice(double price)
        {
            return "£" + price.ToString("0.00");
        }
        public static List<CategoryViewPath> GetCategoryPath(ApplicationDbContext db, string CatPath)
        {
            string[] aPath;
            aPath = CatPath.Split(">", StringSplitOptions.None);
            var Categories = db.Category.ToList();
            List<CategoryViewPath> PagePath = new List<CategoryViewPath>();

            if (aPath.Length > 0)
            {
                foreach (var item in aPath)
                {
                    var Cat = Categories.Where(a => a.Code == item).FirstOrDefault();
                    if (Cat != null)
                    {
                        PagePath.Add(new CategoryViewPath { Code = Cat.Code == "ROOT" ? "" : Cat.Code, Name = Cat.Code == "ROOT" ? "Home" : Cat.Name });
                    }
                }
            }
            return PagePath;
        }

    }
}
