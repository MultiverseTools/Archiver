// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-28-11:07

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档保存的元素
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ArchiverElementAttribute : Attribute
    {
        
    }
}