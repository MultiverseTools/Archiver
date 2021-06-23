// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-23-16:47

using System;

namespace EFAS.Archiver
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ArchiverElementUpdateAttribute : Attribute
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public readonly uint Version;

        /// <summary>
        /// 升级后字段名称
        /// 不填: 使用原来的名称
        /// </summary>
        public readonly string UpdateFieldName;

        /// <summary>
        /// 升级使用方法
        /// </summary>
        public readonly string UpdateFunctionName;

        /// <summary>
        /// 更新存档元素名称
        /// </summary>
        /// <param name="_version">生效版本号</param>
        /// <param name="_updateFieldName">升级后字段名称</param>
        /// <param name="_updateFunctionName">升级存档调用函数</param>
        public ArchiverElementUpdateAttribute(uint _version, string _updateFieldName, string _updateFunctionName)
        {
            Version            = _version;
            UpdateFieldName    = _updateFieldName;
            UpdateFunctionName = _updateFunctionName;
        }

        /// <summary>
        /// 更新存档元素名称
        /// </summary>
        /// <param name="_version">生效版本号</param>
        /// <param name="_updateFunctionName">升级存档调用函数</param>
        public ArchiverElementUpdateAttribute(uint _version, string _updateFunctionName)
        {
            Version            = _version;
            UpdateFunctionName = _updateFunctionName;
        }
    }
}