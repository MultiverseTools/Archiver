// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-24-12:21

using System;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档升级
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class ArchiverUpgradeAttribute : Attribute
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public readonly string Version;

        /// <summary>
        /// 升级使用方法
        /// </summary>
        public readonly string UpgradeFunctionName;

        /// <summary>
        /// 更新存档元素名称
        /// </summary>
        /// <param name="_version">生效版本号. 小于版本号都需要调用对应的方法</param>
        /// <param name="_upgradeFunctionName">升级存档调用函数</param>
        public ArchiverUpgradeAttribute(string _version, string _upgradeFunctionName)
        {
            Version            = _version;
            UpgradeFunctionName = _upgradeFunctionName;
        }
    }
}