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
        /// <summary>
        /// 用于存档操作的元素名称数组(默认: Class/Struct中带[ArchiverElementAttribute]都会用于存档)
        /// 
        /// 此数组使用在Class/Struct(自定义, 除Unity内置)的存档元素上, 在数组中填写的名称才会被用于存档.
        /// 使用情景示例: 
        /// 1) A在保存存档时,只会保存Data.Value1, B在保存存档时, 只会保存Data.Value2
        /// public class A
        /// {
        ///     [ArchiverElement(UsingNameArray = new[] {nameof(Data.Value1))]
        ///     public Data Data;
        /// }
        ///
        /// public class B
        /// {
        ///     [ArchiverElement(UsingNameArray = new[] {nameof(Data.Value2))]
        ///     public Data Data;
        /// }
        ///
        /// public class C
        /// {
        ///     [ArchiverElement]
        ///     public Data Data;
        /// }
        ///
        /// public class Data
        /// {
        ///     [ArchiverElement] // 注意: 使用可选模式, 标签不能省略
        ///     public int Value1;
        ///     [ArchiverElement] // 注意: 使用可选模式, 标签不能省略
        ///     public int Value2;
        /// }
        /// 
        /// </summary>
        public string[] UsingElementNameArray;

        public ArchiverElementAttribute(params string[] _usingElementNames)
        {
            UsingElementNameArray = new string[_usingElementNames.Length];
            Array.Copy(_usingElementNames, UsingElementNameArray, _usingElementNames.Length);
        }
    }
}