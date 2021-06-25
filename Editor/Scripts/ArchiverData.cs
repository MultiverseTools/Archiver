// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-20:23

using System;
using System.Collections.Generic;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档数据
    /// </summary>
    public class ArchiverData
    {
        /// <summary>
        /// 生成目录
        /// </summary>
        public string GenerateFoldPath;

        /// <summary>
        /// 存档类型
        /// </summary>
        public Type Type;

        /// <summary>
        /// 存档元素集合
        /// </summary>
        public List<ArchiverContentData> ArchiverContentSet;
    }
}