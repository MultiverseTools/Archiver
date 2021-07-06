// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-13:53

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档目标的数据
    /// </summary>
    public struct ArchiverContentData
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName;

        /// <summary>
        /// 存档数据类型
        /// </summary>
        public Type Type;
    }
}