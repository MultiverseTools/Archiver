// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-28-10:46

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容过滤转换
    /// </summary>
    public class ArchiverElementFilterConvert : JsonConverter
    {
        /// <summary>
        /// 类型中字段/属性是否有ArchiverElementAttribute
        /// </summary>
        private static readonly Dictionary<Type, bool> s_archiverElementMap = new Dictionary<Type, bool>();

        /// <summary>
        /// 过滤信息
        /// string: path
        /// FilterData: 过滤信息
        /// </summary>
        private static readonly Dictionary<string, FilterData> s_filterDataMap = new Dictionary<string, FilterData>();

        public override void WriteJson(JsonWriter _writer, object? _value, JsonSerializer _serializer)
        {
            if (_value == null)
            {
                _writer.WriteNull();
                return;
            }
            var valueType = _value.GetType();
            var path = _writer.Path;
            // 获取过滤信息
            s_filterDataMap.TryGetValue(path, out var filterData);
            _writer.WriteStartObject();
            // 存档操作所有字段
            foreach (var fieldInfo in valueType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                // 过滤不设置ArchiverElementAttribute的字段
                var archiverElementAttribute = fieldInfo.GetCustomAttribute<ArchiverElementAttribute>();
                if (archiverElementAttribute == null) continue;

                // 存在需要过滤的信息
                if (filterData != null
                    // 不包含当前处理的字段名称
                && !filterData.UsingElementName.Contains(fieldInfo.Name))
                {
                    continue;
                }

                if (archiverElementAttribute.UsingElementNameArray.Length > 0 && CheckCanCovert(fieldInfo.FieldType))
                {
                    _SetFilterData(_BuildElementPath(fieldInfo.Name), archiverElementAttribute.UsingElementNameArray);
                }

                // 保存字段值
                _WriteValue(fieldInfo.Name, fieldInfo.GetValue(_value));
            }

            // 存档操作所有属性
            foreach (var propertyInfo in valueType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // 过滤不设置ArchiverElementAttribute的属性
                var archiverElementAttribute = propertyInfo.GetCustomAttribute<ArchiverElementAttribute>();
                if (archiverElementAttribute == null) continue;

                // 存在需要过滤的信息
                if (filterData != null
                    // 不包含当前处理的属性名称
                && !filterData.UsingElementName.Contains(propertyInfo.Name))
                {
                    continue;
                }

                if (archiverElementAttribute.UsingElementNameArray.Length > 0 && CheckCanCovert(propertyInfo.PropertyType))
                {
                    _SetFilterData(_BuildElementPath(propertyInfo.Name), archiverElementAttribute.UsingElementNameArray);
                }

                // 保存属性值
                _WriteValue(propertyInfo.Name, propertyInfo.GetValue(_value));
            }

            _writer.WriteEndObject();

            // 移除处理完成后的信息
            if (filterData != null)
            {
                s_filterDataMap.Remove(path);
            }

            // 写入存档信息
            void _WriteValue(string _name, object _object)
            {
                _writer.WritePropertyName(_name);
                if (_object == null)
                {
                    _writer.WriteNull();
                    Debug.LogError($"\"{valueType.FullName}.{_name}\"保存null.");
                }
                else
                {
                    _serializer.Serialize(_writer, _object);
                }
            }

            // 构建存档元素路径
            string _BuildElementPath(string _elementPath)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return _elementPath;
                }
                else
                {
                    return $"{path}.{_elementPath}";
                }
            }

            // 设置过滤信息
            void _SetFilterData(string _path, string[] _usingElementNameArray)
            {
                s_filterDataMap.Add(_path, new FilterData()
                {
                    UsingElementName = new HashSet<string>(_usingElementNameArray)
                });
            }
        }

        public override object? ReadJson(JsonReader _reader, Type _objectType, object? _existingValue, JsonSerializer _serializer)
        {
            // 反序列化不需要实现
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type _objectType)
        {
            // 正在处理序列化阶段
            var isCanConvert = ArchiverManager.ProcessStatus == ArchiverManager.PROCESS_STATUS.SAVING;
            isCanConvert &= CheckCanCovert(_objectType);
            return isCanConvert;
        }

        /// <summary>
        /// 过滤信息
        /// </summary>
        private class FilterData
        {
            /// <summary>
            /// 需要存档操作的元素名称
            /// </summary>
            public HashSet<string> UsingElementName;
        }

        /// <summary>
        /// true: 能被Convert
        /// </summary>
        /// <param name="_type">检测类型</param>
        /// <returns></returns>
        private static bool CheckCanCovert(Type _type)
        {
            if (_type.IsPrimitive
            || _type == typeof(string)
            || _type == typeof(decimal)
            || _type.IsEnum)
            {
                return false;
            }
            if (!s_archiverElementMap.TryGetValue(_type, out var isCanConvert))
            {
                // Class/Struct中有且至少有一个字段带[ArchiverElementAttribute]
                isCanConvert = false;

                foreach (var fieldInfo in _type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    // 判断字段是否带ArchiverElementAttribute
                    isCanConvert = fieldInfo.IsDefined(typeof(ArchiverElementAttribute));
                    if (isCanConvert) break;
                }

                if (!isCanConvert)
                {
                    foreach (var propertyInfo in _type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        // 判断属性是否带ArchiverElementAttribute
                        isCanConvert = propertyInfo.IsDefined(typeof(ArchiverElementAttribute));
                        if (isCanConvert) break;
                    }
                }

                s_archiverElementMap.Add(_type, isCanConvert);
            }
            return isCanConvert;
        }
    }
}