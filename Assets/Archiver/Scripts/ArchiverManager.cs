// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:08

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Cysharp.Threading.Tasks;
using EFAS.Archiver.Example;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

        private static int s_batchIndex = 0;

        /// <summary>
        /// 存档内容
        /// </summary>
        private static readonly Archiver s_archiver = new Archiver();

        /// <summary>
        /// 添加存档目标
        /// </summary>
        /// <param name="_object">存档</param>
        public static void AddArchiver(object _object) { s_archiver.Add(_object); }

        /// <summary>
        /// 移除存档目标
        /// </summary>
        /// <param name="_object">存档</param>
        public static void RemoveArchiver(object _object) { s_archiver.Remove(_object); }

        /// <summary>
        /// 保存存档
        /// </summary>
        public static void Save() { Save(s_archiver, s_archiverFilePath); }

        /// <summary>
        /// 保存存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_savePath">存档路径</param>
        public static void Save(Archiver _archiver, string _savePath)
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }

            CollectionData(_archiver, File.CreateText(_savePath)).Forget();
        }

        /// <summary>
        /// 保存数据为Json
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_streamWriter">写入流</param>
        private static async UniTaskVoid CollectionData(Archiver _archiver, StreamWriter _streamWriter)
        {
            using (var jsonTextWriter = new JsonTextWriter(_streamWriter))
            {
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
                            await CheckWait();
                        }

                        jsonTextWriter.WriteEndArray();
                    }
                }

                jsonTextWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// 检测是否需要等待
        /// </summary>
        private static async UniTask CheckWait()
        {
            if (++s_batchIndex % k_batchCount == 0)
            {
                s_batchIndex = 0;
                await UniTask.WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public static Archiver Load()
        {
            var archiver = new Archiver();

            if (File.Exists(s_archiverFilePath))
            {
                var propertyName = string.Empty;
                var json         = File.ReadAllText(s_archiverFilePath);
                var reader       = new JsonTextReader(new StringReader(json));

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            propertyName = (string) reader.Value;

                            // 版本号
                            if (propertyName == nameof(Archiver.Version))
                            {
                                var versionString = reader.ReadAsString();

                                if (!string.IsNullOrEmpty(versionString))
                                {
                                    archiver.Version = new Version(versionString);
                                }
                            }

                            break;

                        case JsonToken.StartArray:
                            // Debug.Log($"Token: {reader.TokenType}, Depth: {reader.Depth}, LineNumber: {reader.LineNumber}, LinePosition: {reader.LinePosition}");
                            reader.Read();

                            while (reader.TokenType != JsonToken.EndArray)
                            {
                                // TokenType: StartObject
                                var startPosition = reader.LinePosition;
                                reader.Skip();
                                // TokenType: EndObject
                                var endPosition           = reader.LinePosition + 1;
                                var deserializeObjectType = Archiver.s_fieldTypeMap[propertyName];
                                var deserializeObject     = JsonConvert.DeserializeObject(json.Substring(startPosition - 1, endPosition - startPosition), deserializeObjectType);

                                foreach (var archiverUpdateAttribute in deserializeObjectType.GetCustomAttributes<ArchiverUpgradeAttribute>())
                                {
                                    if (archiver.Version < Version.Parse(archiverUpdateAttribute.Version))
                                    {
                                        var upgradeMethod     = deserializeObjectType.GetMethod(archiverUpdateAttribute.UpgradeFunctionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                                        var isUpgradeComplete = upgradeMethod != null;

                                        if (isUpgradeComplete)
                                        {
                                            var parameterInfoArray = upgradeMethod.GetParameters();
                                            isUpgradeComplete = parameterInfoArray.Length == 1 && parameterInfoArray[0].ParameterType == deserializeObjectType;

                                            if (isUpgradeComplete)
                                            {
                                                deserializeObject = upgradeMethod.Invoke(null,
                                                    new[]
                                                    {
                                                        deserializeObject
                                                    });

                                                isUpgradeComplete = deserializeObject != null;
                                            }
                                        }

                                        if (!isUpgradeComplete)
                                        {
                                            throw new Exception($"\"{deserializeObjectType.FullName}.{archiverUpdateAttribute.UpgradeFunctionName}\"升级方法错误或返回值为null.任选一种方法:\n"
                                                              + $"private static {deserializeObjectType.Name} {archiverUpdateAttribute.UpgradeFunctionName}({deserializeObjectType.Name} _source)\n"
                                                              + $"public static {deserializeObjectType.Name} {archiverUpdateAttribute.UpgradeFunctionName}({deserializeObjectType.Name} _source)");
                                        }
                                    }
                                }

                                archiver.Add(deserializeObject);
                                // 读取下一个
                                reader.Read();
                            }

                            break;
                    }
                }
            }

            return archiver;
        }
    }
}