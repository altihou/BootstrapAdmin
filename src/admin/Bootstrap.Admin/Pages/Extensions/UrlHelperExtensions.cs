﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace Bootstrap.Admin.Pages.Extensions
{
    /// <summary>
    /// Url 地址辅助操作类
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// 转换为 Blazor 地址 ~/Admin/Index => /Admin/Index
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToBlazorLink(this string url) => url.TrimStart('~');

        /// <summary>
        /// 转化为 Blazor 菜单地址 ~/Admin/Index => /Pages/Admin/Index
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToBlazorMenuUrl(this string url) => url.Replace("~", "Pages");

        /// <summary>
        /// 转化为 Mvc 菜单地址 /Pages/Admin/Index => ~/Admin/Index
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToMvcMenuUrl(this string url)
        {
            var index = new List<string>() { "/Pages", "/Pages/Admin" };
            return index.Any(u => u.Contains(url, System.StringComparison.OrdinalIgnoreCase)) ? "~/Admin/Index" : url.Replace("/Pages", "~");
        }

        /// <summary>
        /// 转换为 Blazor 地址 /Admin/Index => {PathBase}/Admin/Index
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToBlazorLink(this NavigationManager? nav, string url) => $"{nav?.BaseUri}{url.TrimStart('/')}";
    }
}
