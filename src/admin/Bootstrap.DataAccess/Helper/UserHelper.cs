﻿using Bootstrap.Security;
using Bootstrap.Security.DataAccess;
using Longbow.Cache;
using Longbow.GiteeAuth;
using Longbow.GitHubAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Bootstrap.DataAccess
{
    /// <summary>
    /// 用户表相关操作类
    /// </summary>
    public static class UserHelper
    {
        /// <summary>
        /// 获取所有用户缓存数据键值
        /// </summary>
        public const string RetrieveUsersDataKey = "UserHelper-RetrieveUsers";
        /// <summary>
        /// 通过角色ID获取所有用户缓存数据键值
        /// </summary>
        public const string RetrieveUsersByRoleIdDataKey = "UserHelper-RetrieveUsersByRoleId";
        /// <summary>
        /// 通过部门ID获取所有用户缓存数据键值
        /// </summary>
        public const string RetrieveUsersByGroupIdDataKey = "UserHelper-RetrieveUsersByGroupId";
        /// <summary>
        /// 获取所有新用户缓存数据键值
        /// </summary>
        public const string RetrieveNewUsersDataKey = "UserHelper-RetrieveNewUsers";
        /// <summary>
        /// 通过登录名获取登录用户缓存数据键值
        /// </summary>
        public const string RetrieveUsersByNameDataKey = DbHelper.RetrieveUsersByNameDataKey;

        private static bool UserChecker(User user)
        {
            if (user.Description?.Length > 500) user.Description = user.Description.Substring(0, 500);
            if (user.UserName?.Length > 16) user.UserName = user.UserName.Substring(0, 16);
            if (user.Password?.Length > 16) user.Password = user.Password.Substring(0, 16);
            if (user.DisplayName?.Length > 20) user.DisplayName = user.DisplayName.Substring(0, 20);
            var pattern = @"^[a-zA-Z0-9_@.]*$";
            return user.UserName.IsNullOrEmpty() || Regex.IsMatch(user.UserName, pattern);
        }

        /// <summary>
        /// 查询所有用户
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<User> Retrieves() => CacheManager.GetOrAdd(RetrieveUsersDataKey, key => DbContextManager.Create<User>().Retrieves());

        /// <summary>
        /// 认证方法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="configure"></param>
        /// <returns>返回真表示认证通过</returns>
        public static bool Authenticate(string userName, string password, Action<LoginUser> configure)
        {
            if (!UserChecker(new User { UserName = userName, Password = password })) return false;
            var loginUser = new LoginUser
            {
                UserName = userName,
                LoginTime = DateTime.Now,
                Result = "登录失败"
            };
            configure(loginUser);
            var ret = string.IsNullOrEmpty(userName) ? false : DbContextManager.Create<User>().Authenticate(userName, password);
            if (ret) loginUser.Result = "登录成功";
            LoginHelper.Log(loginUser);
            return ret;
        }

        /// <summary>
        /// 短信验证码认证方法
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <param name="secret"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static bool AuthenticateMobile(string phone, string code, string secret, Action<LoginUser> configure)
        {
            var loginUser = new LoginUser
            {
                UserName = phone,
                LoginTime = DateTime.Now,
                Result = "登录失败"
            };
            configure(loginUser);
            var ret = string.IsNullOrEmpty(phone) ? false : SMSHelper.Validate(phone, code, secret);
            if (ret) loginUser.Result = "登录成功";
            LoginHelper.Log(loginUser);
            return ret;
        }

        /// <summary>
        /// 查询所有的新注册用户
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<User> RetrieveNewUsers() => CacheManager.GetOrAdd(RetrieveNewUsersDataKey, key => DbContextManager.Create<User>().RetrieveNewUsers());

        private static IEnumerable<User> RetrieveConstUsers()
        {
            var users = new string[] { "Admin", "User" };
            return Retrieves().Where(u => users.Any(usr => usr.Equals(u.UserName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="value"></param>
        public static bool Delete(IEnumerable<string> value)
        {
            var admins = RetrieveConstUsers();
            value = value.Where(v => !admins.Any(u => u.Id == v));
            if (!value.Any()) return true;
            var ret = DbContextManager.Create<User>().Delete(value);
            if (ret) CacheCleanUtility.ClearCache(userIds: value, cacheKey: RetrieveUsersByNameDataKey + "*");
            return ret;
        }

        /// <summary>
        /// 保存用户默认App
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static bool SaveApp(string userName, string app)
        {
            var ret = DbContextManager.Create<User>().SaveApp(userName, app);
            if (ret) CacheCleanUtility.ClearCache(cacheKey: $"{RetrieveUsersByNameDataKey}*");
            return ret;
        }

        /// <summary>
        /// 保存新建
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool Save(User user)
        {
            if (!UserChecker(user)) return false;
            if (DictHelper.RetrieveSystemModel() && !string.IsNullOrEmpty(user.Id) && RetrieveConstUsers().Any(u => u.Id == user.Id)) return true;

            var ret = DbContextManager.Create<User>().Save(user);
            if (ret) CacheCleanUtility.ClearCache(userIds: string.IsNullOrEmpty(user.Id) ? new List<string>() : new List<string>() { user.Id }, cacheKey: $"{RetrieveUsersByNameDataKey}*");
            return ret;
        }

        /// <summary>
        /// 更新用户方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static bool Update(string id, string password, string displayName)
        {
            if (!UserChecker(new User { Password = password, DisplayName = displayName })) return false;
            if (DictHelper.RetrieveSystemModel() && RetrieveConstUsers().Any(v => v.Id == id)) return true;

            var ret = DbContextManager.Create<User>().Update(id, password, displayName);
            if (ret) CacheCleanUtility.ClearCache(userIds: string.IsNullOrEmpty(id) ? new List<string>() : new List<string>() { id }, cacheKey: $"{RetrieveUsersByNameDataKey}*");
            return ret;
        }

        /// <summary>
        /// 批准新注册用户方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approvedBy"></param>
        /// <returns></returns>
        public static bool Approve(string id, string approvedBy)
        {
            var ret = DbContextManager.Create<User>().Approve(id, approvedBy);
            if (ret) CacheCleanUtility.ClearCache(userIds: new List<string>() { id });
            return ret;
        }

        /// <summary>
        /// 更改密码方法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public static bool ChangePassword(string userName, string password, string newPass)
        {
            if (!UserChecker(new User { UserName = userName, Password = password })) return false;
            if (DictHelper.RetrieveSystemModel()
                && RetrieveConstUsers().Any(u => userName.Equals(u.UserName, StringComparison.OrdinalIgnoreCase)))
                return true;
            return DbContextManager.Create<User>().ChangePassword(userName, password, newPass);
        }

        /// <summary>
        /// 重置密码方法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <remarks>用户使用忘记密码功能后，管理员在用户管理页面中可以点击重置按钮</remarks>
        /// <returns></returns>
        public static bool ResetPassword(string userName, string password)
        {
            if (!UserChecker(new User { UserName = userName, Password = password })) return false;
            if (DictHelper.RetrieveSystemModel() && RetrieveConstUsers().Any(u => userName.Equals(u.UserName, StringComparison.OrdinalIgnoreCase))) return true;
            var ret = DbContextManager.Create<User>().ResetPassword(userName, password);
            if (ret) CacheCleanUtility.ClearCache(cacheKey: RetrieveUsersDataKey);
            return ret;
        }

        /// <summary>
        /// 忘记密码调用
        /// </summary>
        /// <param name="user"></param>
        public static bool ForgotPassword(ResetUser user)
        {
            var ret = DbContextManager.Create<User>().ForgotPassword(user);
            if (ret) CacheCleanUtility.ClearCache(cacheKey: RetrieveUsersDataKey);
            return ret;
        }

        /// <summary>
        /// 新注册用户拒绝方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rejectBy"></param>
        /// <returns></returns>
        public static bool Reject(string id, string rejectBy)
        {
            var ret = DbContextManager.Create<User>().Reject(id, rejectBy);
            if (ret) CacheCleanUtility.ClearCache(userIds: new List<string>() { id });
            return ret;
        }

        /// <summary>
        /// 通过roleId获取所有用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static IEnumerable<User> RetrievesByRoleId(string roleId) => CacheManager.GetOrAdd(string.Format("{0}-{1}", RetrieveUsersByRoleIdDataKey, roleId), k => DbContextManager.Create<User>().RetrievesByRoleId(roleId), RetrieveUsersByRoleIdDataKey);

        /// <summary>
        /// 通过角色ID保存当前授权用户（插入）
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="userIds">用户ID数组</param>
        /// <returns></returns>
        public static bool SaveByRoleId(string roleId, IEnumerable<string> userIds)
        {
            // 演示模式时禁止修改 Admin 对 Administrators 角色的移除操作
            if (DictHelper.RetrieveSystemModel())
            {
                var adminRole = RoleHelper.Retrieves().FirstOrDefault(r => r.RoleName.Equals("Administrators", StringComparison.OrdinalIgnoreCase)).Id;
                if (roleId.Equals(adminRole, StringComparison.OrdinalIgnoreCase))
                {
                    var adminId = Retrieves().FirstOrDefault(u => u.UserName.Equals("Admin", StringComparison.OrdinalIgnoreCase)).Id;
                    userIds = userIds.Union(new string[] { adminId });
                }
            }

            var ret = DbContextManager.Create<User>().SaveByRoleId(roleId, userIds);
            if (ret) CacheCleanUtility.ClearCache(userIds: userIds, roleIds: new List<string>() { roleId });
            return ret;
        }

        /// <summary>
        /// 通过groupId获取所有用户
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static IEnumerable<User> RetrievesByGroupId(string groupId) => CacheManager.GetOrAdd(string.Format("{0}-{1}", RetrieveUsersByGroupIdDataKey, groupId), k => DbContextManager.Create<User>().RetrievesByGroupId(groupId), RetrieveUsersByRoleIdDataKey);

        /// <summary>
        /// 通过部门ID保存当前授权用户（插入）
        /// </summary>
        /// <param name="groupId">GroupID</param>
        /// <param name="userIds">用户ID数组</param>
        /// <returns></returns>
        public static bool SaveByGroupId(string groupId, IEnumerable<string> userIds)
        {
            var ret = DbContextManager.Create<User>().SaveByGroupId(groupId, userIds);
            if (ret) CacheCleanUtility.ClearCache(userIds: userIds, groupIds: new List<string>() { groupId });
            return ret;
        }

        /// <summary>
        /// 根据用户名修改用户头像
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="iconName"></param>
        /// <returns></returns>
        public static bool SaveUserIconByName(string userName, string iconName)
        {
            var ret = DbContextManager.Create<User>().SaveUserIconByName(userName, iconName);
            if (ret) CacheCleanUtility.ClearCache(cacheKey: $"{RetrieveUsersByNameDataKey}*");
            return ret;
        }

        /// <summary>
        /// 保存显示名称方法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static bool SaveDisplayName(string userName, string displayName)
        {
            if (!UserChecker(new User { UserName = userName, DisplayName = displayName })) return false;
            var ret = DbContextManager.Create<User>().SaveDisplayName(userName, displayName);
            if (ret) CacheCleanUtility.ClearCache(cacheKey: $"{RetrieveUsersByNameDataKey}*");
            return ret;
        }

        /// <summary>
        /// 根据用户名更改用户皮肤
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cssName"></param>
        /// <returns></returns>
        public static bool SaveUserCssByName(string userName, string cssName)
        {
            var ret = DbContextManager.Create<User>().SaveUserCssByName(userName, cssName);
            if (ret) CacheCleanUtility.ClearCache(cacheKey: $"{RetrieveUsersByNameDataKey}*");
            return ret;
        }

        /// <summary>
        /// 通过登录名获取登录用户方法
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static BootstrapUser RetrieveUserByUserName(IIdentity identity) => CacheManager.GetOrAdd(string.Format("{0}-{1}", RetrieveUsersByNameDataKey, identity.Name), k =>
        {
            var userName = identity.Name;
            var proxyList = new List<Func<string, BootstrapUser>>();

            // 本地数据库认证
            proxyList.Add(DbContextManager.Create<User>().RetrieveUserByUserName);

            // Gitee 认证
            if (identity.AuthenticationType == GiteeDefaults.AuthenticationScheme) proxyList.Add(OAuthHelper.RetrieveUserByUserName<GiteeOptions>);

            // GitHub 认证
            if (identity.AuthenticationType == GitHubDefaults.AuthenticationScheme) proxyList.Add(OAuthHelper.RetrieveUserByUserName<GitHubOptions>);

            BootstrapUser user = null;
            foreach (var p in proxyList)
            {
                user = p.Invoke(userName);
                if (user != null) break;
            }
            return user;
        }, RetrieveUsersByNameDataKey);

        /// <summary>
        /// 通过登录账号获得用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static ResetUser RetrieveResetUserByUserName(string userName) => DbContextManager.Create<ResetUser>().RetrieveUserByUserName(userName);

        /// <summary>
        /// 通过登录账户获得重置密码原因
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<DateTime, string>> RetrieveResetReasonsByUserName(string userName) => DbContextManager.Create<ResetUser>().RetrieveResetReasonsByUserName(userName);
    }
}
