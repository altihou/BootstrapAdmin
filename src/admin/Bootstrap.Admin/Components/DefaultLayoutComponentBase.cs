﻿using Bootstrap.Admin.Extensions;
using Bootstrap.Admin.Models;
using Bootstrap.Admin.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.JSInterop;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bootstrap.Admin.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultLayoutComponentBase : LayoutComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = new ServerAuthenticationStateProvider();

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IJSRuntime? JSRuntime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NavigatorBarModel Model { get; set; } = new NavigatorBarModel("");

        /// <summary>
        /// 
        /// </summary>
        public SideBar? SideBar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Header? Header { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        protected bool IsAdmin { get; set; }

        /// <summary>
        /// 获得/设置 系统首页
        /// </summary>
        public string HomeUrl { get; protected set; } = "/Pages";

        /// <summary>
        /// 获得/设置 当前请求路径
        /// </summary>
        protected string RequestUrl { get; set; } = "";

        /// <summary>
        /// OnInitializedAsync 方法
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!state.User.Identity.IsAuthenticated)
            {
                NavigationManager?.NavigateTo("/Account/Login?returnUrl=" + WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery));
            }
            else
            {
                RequestUrl = new UriBuilder(NavigationManager?.Uri ?? "").Path;
                Model = new NavigatorBarModel(state.User.Identity.Name, RequestUrl.ToMvcMenuUrl());
                IsAdmin = state.User.IsInRole("Administrators");
                UserName = state.User.Identity.Name ?? "";
                DisplayName = Model.DisplayName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) ResetSideBar();
        }

        /// <summary>
        /// 更新侧边栏方法
        /// </summary>
        public void ResetSideBar()
        {
            RequestUrl = new UriBuilder(NavigationManager?.Uri ?? "").Path;
            Model = new NavigatorBarModel(UserName, RequestUrl.ToMvcMenuUrl());
            SideBar?.Update(Model);
        }
    }
}
