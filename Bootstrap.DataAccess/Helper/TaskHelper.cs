﻿using Longbow.Cache;
using Longbow.Data;
using System.Collections.Generic;

namespace Bootstrap.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public const string RetrieveTasksDataKey = "TaskHelper-RetrieveTasks";

        /// <summary>
        /// 查询所有任务
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Task> Retrieves() => CacheManager.GetOrAdd(RetrieveTasksDataKey, key => DbContextManager.Create<Task>().Retrieves());
    }
}
