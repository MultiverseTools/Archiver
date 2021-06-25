﻿// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:08

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档管理
    /// </summary>
    public static class ArchiverManager
    {
        /// <summary>
        /// 游戏版本号
        /// </summary>
        public static readonly Version s_gameVersion = new Version(0, 0, 0, 0);

        /// <summary>
        /// 存档位置
        /// </summary>
        public static string s_archiverFilePath => Path.Combine(Application.dataPath, "Archiver.json");

        /// <summary>
        /// 每次处理的保存数据的数量
        /// </summary>
        private const int k_batchCount = 10;

        /// <summary>
        /// 当前处理的index
        /// </summary>
        private static int s_batchIndex = 0;

        /// <summary>
        /// 存档内容
        /// </summary>
        public static Archiver s_archiver = new Archiver();

        /// <summary>
        /// 添加存档内容
        /// </summary>
        /// <param name="_object">内容</param>
        public static void AddArchiver(object _object) { s_archiver.Add(_object); }

        /// <summary>
        /// 移除存档内容
        /// </summary>
        /// <param name="_object">内容</param>
        public static void RemoveArchiver(object _object) { s_archiver.Remove(_object); }
        
        /// <summary>
        /// 保存存档到<see cref="s_archiver"/>
        /// </summary>
        /// <param name="_completeAction">存档完成时回调</param>
        public static void SaveArchiver(Action _completeAction) => SaveArchiver(s_archiver, s_archiverFilePath, _completeAction);

        /// <summary>
        /// 保存存档到<see cref="s_archiver"/>
        /// </summary>
        public static UniTask SaveArchiver() => SaveArchiver(s_archiver, s_archiverFilePath);
        
        /// <summary>
        /// 保存指定存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">存档路径</param>
        /// <param name="_completeAction">存档完成时回调</param>
        public static void SaveArchiver(Archiver _archiver, string _path, Action _completeAction)
        {
            _SaveArchiver().Forget();

            async UniTaskVoid _SaveArchiver()
            {
                await SaveArchiver(_archiver, _path);
                _completeAction?.Invoke();
            }
        }

        /// <summary>
        /// 保存指定存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">存档路径</param>
        public static UniTask SaveArchiver(Archiver _archiver, string _path)
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }

            return CollectionData(_archiver, _path);
        }

        /// <summary>
        /// 读取存档到<see cref="s_archiver"/>
        /// </summary>
        /// <param name="_completeAction">读取完成回调</param>
        public static void LoadArchiver(Action _completeAction) => LoadArchiver(s_archiver, s_archiverFilePath, _completeAction);
        
        /// <summary>
        /// 读取存档到<see cref="s_archiver"/>
        /// </summary>
        public static UniTask LoadArchiver() => LoadArchiver(s_archiver, s_archiverFilePath);

        /// <summary>
        /// 读取存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">读档路径</param>
        /// <param name="_completeAction">读取完成回调</param>
        public static void LoadArchiver(Archiver _archiver, string _path, Action _completeAction)
        {
            _LoadArchiver().Forget();

            async UniTaskVoid _LoadArchiver()
            {
                await LoadArchiver(_archiver, _path);
                _completeAction?.Invoke();
            }
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">读档路径</param>
        public static async UniTask LoadArchiver(Archiver _archiver, string _path)
        {
            if (File.Exists(_path))
            {
                var propertyName = string.Empty;
                // 读取文件json
                var json   = File.ReadAllText(_path);
                var reader = new JsonTextReader(new StringReader(json));

                // 读取所有json内容
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            propertyName = (string) reader.Value;

                            // 版本号
                            if (propertyName == nameof(Archiver.Version))
                            {
                                // 设置版本号string
                                var versionString = reader.ReadAsString();

                                if (!string.IsNullOrEmpty(versionString))
                                {
                                    // 设置版本号
                                    _archiver.Version = new Version(versionString);
                                }
                            }

                            break;

                        case JsonToken.StartArray:
                            // 读取Array第一个元素
                            reader.Read();
                            // 升级后的版本号
                            Version upgradeVersion = null;

                            while (reader.TokenType != JsonToken.EndArray)
                            {
                                /*
                                 * 获取反序列化内容
                                 */
                                // TokenType: StartObject
                                // 记录开始位置
                                var startPosition = reader.LinePosition - 1;
                                // 添加 Object内容
                                reader.Skip();
                                // TokenType: EndObject
                                // 记录结束位置
                                var endPosition = reader.LinePosition + 1;
                                // 通过字段名称获取对应的类型
                                if (string.IsNullOrEmpty(propertyName)) throw new Exception($"属性名称不能为空");
                                var deserializeObjectType = Archiver.s_fieldTypeMap[propertyName];
                                // 获取json对应的实体
                                var deserializeObject = JsonConvert.DeserializeObject(json.Substring(startPosition, endPosition - startPosition - 1), deserializeObjectType);
                                await CheckWaitBatch();

                                /*
                                 * 存档升级
                                 */
                                // 获取所有升级Attribute
                                var archiverUpgradeAttributes = new List<ArchiverUpgradeAttribute>(deserializeObjectType.GetCustomAttributes<ArchiverUpgradeAttribute>());
                                var checkVersion              = new Version(0, 0, 0, 0);

                                // 检测Attribute是否按照从低到高版本号顺序
                                foreach (var archiverUpgradeAttribute in archiverUpgradeAttributes)
                                {
                                    var version = Version.Parse(archiverUpgradeAttribute.Version);

                                    if (checkVersion <= version)
                                    {
                                        checkVersion = version;
                                    }
                                    else
                                    {
                                        throw new Exception($"\"{deserializeObjectType.FullName}\"存档升级\"{nameof(ArchiverUpgradeAttribute)}\"版本号必须从低到高排列.");
                                    }
                                }

                                // 升级存档
                                foreach (var archiverUpgradeAttribute in archiverUpgradeAttributes)
                                {
                                    var version = Version.Parse(archiverUpgradeAttribute.Version);

                                    // 判断存档版本是否升级代码
                                    if (_archiver.Version < version)
                                    {
                                        // 获取升级方法
                                        var upgradeMethod     = deserializeObjectType.GetMethod(archiverUpgradeAttribute.UpgradeFunctionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                                        var isUpgradeComplete = upgradeMethod != null;

                                        if (isUpgradeComplete)
                                        {
                                            // 判断升级函数参数
                                            var parameterInfoArray = upgradeMethod.GetParameters();
                                            // 参数必须为一个且类型为存档类型
                                            isUpgradeComplete = parameterInfoArray.Length == 1 && parameterInfoArray[0].ParameterType == deserializeObjectType;

                                            if (isUpgradeComplete)
                                            {
                                                // 调用升级函数
                                                deserializeObject = upgradeMethod.Invoke(null,
                                                    new[]
                                                    {
                                                        deserializeObject
                                                    });

                                                isUpgradeComplete = deserializeObject != null;
                                            }
                                        }

                                        if (isUpgradeComplete)
                                        {
                                            if (upgradeVersion != version)
                                            {
                                                // 升级版本号
                                                upgradeVersion = new Version(version.ToString());
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception($"\"{deserializeObjectType.FullName}.{archiverUpgradeAttribute.UpgradeFunctionName}\"升级方法错误或返回值为null.任选一种方法:\n"
                                                              + $"private static {deserializeObjectType.Name} {archiverUpgradeAttribute.UpgradeFunctionName}({deserializeObjectType.Name} _source)\n"
                                                              + $"public static {deserializeObjectType.Name} {archiverUpgradeAttribute.UpgradeFunctionName}({deserializeObjectType.Name} _source)");
                                        }
                                    }
                                }

                                // 保存实体
                                _archiver.Add(deserializeObject);
                                // 读取下一个
                                reader.Read();
                            }

                            if (upgradeVersion != null)
                            {
                                _archiver.Version = upgradeVersion;
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 手机存档类数据为Json, 并保存到指定路径中
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_savePath">存档路径</param>
        private static async UniTask CollectionData(Archiver _archiver, string _savePath)
        {
            using (var jsonTextWriter = new JsonTextWriter(File.CreateText(_savePath)))
            {
                // 开始写入json
                jsonTextWriter.WriteStartObject();

                // 遍历写入所有字段
                foreach (var fieldInfo in typeof(Archiver).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    jsonTextWriter.WritePropertyName(fieldInfo.Name);
                    var fieldValue = fieldInfo.GetValue(_archiver);

                    // 保存版本号
                    if (fieldInfo.Name == nameof(Archiver.Version))
                    {
                        jsonTextWriter.WriteRawValue(JsonConvert.SerializeObject(fieldValue, Formatting.None));
                    }
                    // 当前只有List类型数据
                    else
                    {
                        jsonTextWriter.WriteStartArray();
                        var iterator = (IEnumerable) fieldValue;

                        foreach (var item in iterator)
                        {
                            jsonTextWriter.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.None));

                            // 同时处理量不能过大
                            await CheckWaitBatch();
                        }

                        jsonTextWriter.WriteEndArray();
                    }
                }

                jsonTextWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// 等待合批处理
        /// </summary>
        private static async UniTask CheckWaitBatch()
        {
            if (++s_batchIndex % k_batchCount == 0)
            {
                s_batchIndex = 0;
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}