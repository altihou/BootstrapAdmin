﻿namespace Bootstrap.DataAccess
{
    /// <summary>
    /// 字典表实体
    /// </summary>
    public class Dict
    {
        /// <summary>
        /// 字典主键 数据库自增
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 1表示系统使用，0表示用户自定义 默认为1
        /// </summary>
        public int Define { get; set; }
    }
}
