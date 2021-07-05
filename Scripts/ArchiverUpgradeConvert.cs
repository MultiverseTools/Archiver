// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-07-01-13:26

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace EFAS.Archiver
{
    /// <summary>
    /// 升级存档
    /// 在读取Json时执行
    /// </summary>
    public class ArchiverUpgradeConvert : JsonConverter
    {
        private static readonly HashSet<Type> s_readingTypeSet = new HashSet<Type>();

        public override void WriteJson(JsonWriter _writer, object? _value, JsonSerializer _serializer)
        {
            // 序列化不需要实现
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader _reader, Type _objectType, object? _existingValue, JsonSerializer _serializer)
        {
            if (_reader.TokenType == JsonToken.Null)
            {
                return null;
            }

#if UNITY_EDITOR
            // 升级检查
            _objectType.CheckUpgradeValid();
#endif
            var deserializeObject = _serializer.Deserialize(_reader, _objectType);
            s_readingTypeSet.Remove(_objectType);

            // 升级存档
            foreach (var archiverUpgradeAttribute in _objectType.GetCustomAttributes<ArchiverUpgradeAttribute>())
            {
                var version = Version.Parse(archiverUpgradeAttribute.Version);

                // 判断存档版本是否升级代码
                if (ArchiverManager.LoadingArchiverVersion < version)
                {
                    // 获取升级方法
                    var upgradeMethod = _objectType.GetMethod(archiverUpgradeAttribute.UpgradeFunctionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    // 调用升级函数
                    deserializeObject = upgradeMethod.Invoke(null,
                        new[]
                        {
                            deserializeObject
                        });

                    // 升级版本号
                    if (ArchiverManager.LoadingHighestVersion < version)
                    {
                        ArchiverManager.LoadingHighestVersion = new Version(version.ToString());
                    }

                    if (deserializeObject == null)
                    {
                        throw new Exception($"\"{_objectType.FullName}.{archiverUpgradeAttribute.UpgradeFunctionName}\"返回值为null.");
                    }
                }
            }

            return deserializeObject;
        }

        public override bool CanConvert(Type _objectType)
        {
            var isCanConvert = ArchiverManager.ProcessStatus == ArchiverManager.PROCESS_STATUS.LOADING;
            if (isCanConvert)
            {
                // TODO 优化保存类型, 不用每次都判断
                // Class/Struct中有带[ArchiverUpgradeAttribute]
                isCanConvert = _objectType.IsDefined(typeof(ArchiverUpgradeAttribute), false);
            }
            if (isCanConvert)
            {
                isCanConvert = !s_readingTypeSet.Contains(_objectType);
                if (isCanConvert)
                {
                    s_readingTypeSet.Add(_objectType);
                }
            }
            return isCanConvert;
        }
    }
}