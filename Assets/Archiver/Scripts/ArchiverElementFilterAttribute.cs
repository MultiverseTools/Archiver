// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-29-9:07

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容只能保存ArchiverElementAttribute标记的内容
    /// 所有需要保存为JSON内容的Class/Struct都需要添加此标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ArchiverElementFilterAttribute : Attribute
    {
    }
}