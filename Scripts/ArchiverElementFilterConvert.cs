﻿// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-28-10:46

using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容过滤转换
    /// </summary>
    public class ArchiverElementFilterConvert : JsonConverter
    {
        public override void WriteJson(JsonWriter _writer, object? _value, JsonSerializer _serializer)
        {
            if (_value == null)
            {
                _writer.WriteNull();
                return;
            }

            var valueType = _value.GetType();
            _writer.WriteStartObject();

            foreach (var memberInfo in valueType.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                // 过滤不设置ArchiverElementAttribute的字段
                if (!Attribute.IsDefined(memberInfo, typeof(ArchiverElementAttribute))) continue;
                if (memberInfo is FieldInfo fieldInfo)
                {
                    // 保存字段值
                    _WriteValue(memberInfo.Name, fieldInfo.GetValue(_value));
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    // 保存属性值
                    _WriteValue(memberInfo.Name, propertyInfo.GetValue(_value));
                }
            }

            _writer.WriteEndObject();

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
                    _writer.WriteRawValue(JsonConvert.SerializeObject(_object, _serializer.Formatting));
                }
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
            if (isCanConvert)
            {
                // TODO 优化使用字段保存类型, 不用每次都判断
                // Class/Struct中有且至少有一个字段带[ArchiverElementAttribute]
                isCanConvert = false;
                foreach (var memberInfo in _objectType.GetMembers(BindingFlags.Instance | BindingFlags.Public))
                {
                    // 判断属性/字段是否带ArchiverElementAttribute
                    if (memberInfo.MemberType == MemberTypes.Field
                    || memberInfo.MemberType == MemberTypes.Property)
                    {
                        isCanConvert = Attribute.IsDefined(memberInfo, typeof(ArchiverElementAttribute));
                    }
                    if (isCanConvert) break;
                }
            }
            return isCanConvert;
        }
    }
}