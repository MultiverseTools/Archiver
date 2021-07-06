// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-19:26

using System;
using System.Collections.Generic;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档
    /// </summary>
    public interface IArchiver
    {
        /// <summary>
        /// 存档数据类型字典
        /// </summary>
        Dictionary<string, Type> ArchiverDataTypeMap { get; }

        /// <summary>
        /// 版本号
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="_object">存档元素</param>
        void Add(object _object);

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="_object">存档元素</param>
        void Remove(object _object);

        /// <summary>
        /// 移除所有元素
        /// </summary>
        void Clear();
    }
}