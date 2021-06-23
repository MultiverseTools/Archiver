// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-09-16:59

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ArchiverAttribute : Attribute
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public readonly string GroupName;

        /// <summary>
        /// 设置存档目标
        /// </summary>
        /// <param name="_groupName">存档路径</param>
        public ArchiverAttribute(string _groupName) { GroupName = _groupName; }
    }
}