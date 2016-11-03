﻿using Bootstrap.DataAccess;
using Longbow.Web.Mvc;
using System.Linq;

namespace Bootstrap.Admin.Models
{
    public class QueryMenuOption : PaginationOption
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        public QueryData<Menu> RetrieveData()
        {
            var data = MenuHelper.RetrieveMenus();
            if (!string.IsNullOrEmpty(Name))
            {
                data = data.Where(t => t.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Category))
            {
                data = data.Where(t => t.Category.Contains(Category));
            }
            var ret = new QueryData<Menu>();
            ret.total = data.Count();
            switch (Sort)
            {
                case "Name":
                    data = Order == "asc" ? data.OrderBy(t => t.Name) : data.OrderByDescending(t => t.Name);
                    break;
                case "Order":
                    data = Order == "asc" ? data.OrderBy(t => t.Order) : data.OrderByDescending(t => t.Order);
                    break;
                case "CategoryName":
                    data = Order == "asc" ? data.OrderBy(t => t.CategoryName) : data.OrderByDescending(t => t.CategoryName);
                    break;
                default:
                    break;
            }
            ret.rows = data.Skip(Offset).Take(Limit);
            return ret;
        }
    }
}