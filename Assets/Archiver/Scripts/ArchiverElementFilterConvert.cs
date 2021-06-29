// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-28-10:46

using System;
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
        public override void WriteJson(JsonWriter _writer, object? _value, JsonSerializer _serializer)
        {
            if (_value == null)
            {
                _writer.WriteNull();
                return;
            }

            var valueType = _value.GetType();
            _writer.WriteStartObject();

            foreach (var fieldInfo in valueType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                // 过滤不设置ArchiverElementAttribute的字段
                if (!fieldInfo.IsDefined(typeof(ArchiverElementAttribute), false)) continue;
                // 保存字段
                var fieldValue = fieldInfo.GetValue(_value);
                _writer.WritePropertyName(fieldInfo.Name);
                if (fieldValue == null)
                {
                    _writer.WriteNull();
                    Debug.LogError($"\"{valueType.FullName}.{fieldInfo.Name}\"保存null.");
                }
                else
                {
                    _writer.WriteRawValue(JsonConvert.SerializeObject(fieldValue, _serializer.Formatting));
                }
            }

            _writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader _reader, Type _objectType, object? _existingValue, JsonSerializer _serializer)
        {
            // 反序列化不需要实现
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type _objectType)
        {
            // 标记了ArchiverElementFilterAttribute && 正在处理序列化阶段
            return _objectType.IsDefined(typeof(ArchiverElementFilterAttribute), true) && ArchiverManager.s_processStatus == ArchiverManager.PROCESS_STATUS.SERIALIZE;   
        }
    }
}