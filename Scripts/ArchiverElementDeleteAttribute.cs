// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-23-15:38

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 删除存档元素
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ArchiverElementDeleteAttribute : Attribute
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public readonly uint Version;

        /// <summary>
        /// 更新存档元素数据类型
        /// </summary>
        /// <param name="_version">生效版本号</param>
        public ArchiverElementDeleteAttribute(uint _version) { Version = _version; }
    }
}