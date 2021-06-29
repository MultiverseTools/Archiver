// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-20:08

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档物体
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ArchiverAttribute : Attribute
    {
        /// <summary>
        /// 存档文件位置
        /// </summary>
        public readonly string GenerateFoldPath;

        /// <summary>
        /// 命名空间
        /// 不填: 使用添加处的命名空间
        /// </summary>
        public string Namespace;

        public ArchiverAttribute(string _generateFoldPath)
        {
            GenerateFoldPath = _generateFoldPath;
        }
    }
}