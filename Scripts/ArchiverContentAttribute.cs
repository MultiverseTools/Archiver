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
    public class ArchiverContentAttribute : Attribute
    {
        /// <summary>
        /// 存档类型
        /// </summary>
        public readonly Type ArchiverType;

        /// <summary>
        /// 分组名称
        /// </summary>
        public readonly string GroupName;

        /// <summary>
        /// 设置存档目标
        /// </summary>
        /// <param name="_archiverType">存档类型</param>
        /// <param name="_groupName">存档路径</param>
        public ArchiverContentAttribute(Type _archiverType, string _groupName)
        {
            ArchiverType = _archiverType;
            GroupName        = _groupName;
        }
    }
}