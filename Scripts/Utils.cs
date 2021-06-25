// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-16:35

using System;
using System.Reflection;

namespace EFAS.Archiver
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 检测升级存档功能能否使用
        /// </summary>
        /// <param name="_type"></param>
        /// <exception cref="Exception"></exception>
        public static void CheckUpgradeValid(this Type _type)
        {
            var checkVersion = new Version(0, 0, 0, 0);

            // 检测Attribute是否按照从低到高版本号顺序
            foreach (var archiverUpgradeAttribute in _type.GetCustomAttributes<ArchiverUpgradeAttribute>())
            {
                // 检测版本号
                var version = Version.Parse(archiverUpgradeAttribute.Version);

                if (checkVersion <= version)
                {
                    checkVersion = version;
                }
                else
                {
                    throw new Exception($"\"{_type.FullName}\"存档升级\"{nameof(ArchiverUpgradeAttribute)}\"版本号必须从低到高排列.");
                }

                var isValid = true;
                // 检测升级方法
                var upgradeMethod = _type.GetMethod(archiverUpgradeAttribute.UpgradeFunctionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (upgradeMethod != null)
                {
                    var parameterInfoArray = upgradeMethod.GetParameters();
                    // 参数必须为一个且类型为存档类型
                    isValid = parameterInfoArray.Length == 1 && parameterInfoArray[0].ParameterType == _type;
                }
                else
                {
                    isValid = false;
                }

                if (!isValid)
                {
                    throw new Exception($"\"{_type.FullName}.{archiverUpgradeAttribute.UpgradeFunctionName}\"升级方法错误或返回值为null.任选一种方法:\n"
                                      + $"private static {_type.Name} {archiverUpgradeAttribute.UpgradeFunctionName}({_type.Name} _source)\n"
                                      + $"public static {_type.Name} {archiverUpgradeAttribute.UpgradeFunctionName}({_type.Name} _source)");
                }
            }
        }
    }
}