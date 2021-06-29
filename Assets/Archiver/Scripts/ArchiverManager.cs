// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:08

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档管理
    /// </summary>
    public static class ArchiverManager
    {
        /// <summary>
        /// 处理状态
        /// </summary>
        public static PROCESS_STATUS ProcessStatus { get; private set; }

        /// <summary>
        /// 每次处理的保存数据的数量
        /// </summary>
        private const int k_batchCount = 10;

        /// <summary>
        /// 当前处理的index
        /// </summary>
        private static int s_batchIndex = 0;

        /// <summary>
        /// 保存指定存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">存档路径</param>
        /// <param name="_completeAction">存档完成时回调</param>
        public static void SaveArchiver(IArchiver _archiver, string _path, Action _completeAction)
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
        public static UniTask SaveArchiver(IArchiver _archiver, string _path)
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }

            return CollectionData(_archiver, _path);
        }

        /// <summary>
        /// 读取存档并且升级存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">读档路径</param>
        /// <param name="_completeAction">读取完成回调</param>
        public static void LoadArchiver(IArchiver _archiver, string _path, Action _completeAction)
        {
            _LoadArchiver().Forget();

            async UniTaskVoid _LoadArchiver()
            {
                await LoadArchiver(_archiver, _path);
                _completeAction?.Invoke();
            }
        }

        /// <summary>
        /// 读取存档并且升级存档
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_path">读档路径</param>
        public static async UniTask LoadArchiver(IArchiver _archiver, string _path)
        {
            if (!File.Exists(_path))
            {
                throw new Exception($"保存路径不存在\n\"{_path}\"");
            }

            ProcessStatus = PROCESS_STATUS.LOADING;
            var propertyName = string.Empty;
            // 升级后的版本号
            Version upgradeVersion = null;
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
                        if (propertyName == nameof(IArchiver.Version))
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

                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            /*
                             * 获取反序列化内容
                             */
                            // TokenType: StartObject
                            // 记录开始位置
                            var startPosition = reader.LinePosition - 1;
                            // 跳过Object内容
                            reader.Skip();
                            // TokenType: EndObject
                            // 记录结束位置
                            var endPosition = reader.LinePosition + 1;
                            // 通过字段名称获取对应的类型
                            if (string.IsNullOrEmpty(propertyName)) throw new Exception($"属性名称不能为空");
                            var deserializeObjectType = _archiver.ArchiverDataTypeMap[propertyName];
                            // 获取json对应的实体
                            var deserializeObject = JsonConvert.DeserializeObject(json.Substring(startPosition, endPosition - startPosition - 1), deserializeObjectType);
                            await CheckWaitBatch();

                            /*
                             * 存档升级
                             */
#if UNITY_EDITOR
                            deserializeObjectType.CheckUpgradeValid();
#endif
                            // 升级存档
                            foreach (var archiverUpgradeAttribute in deserializeObjectType.GetCustomAttributes<ArchiverUpgradeAttribute>())
                            {
                                var version = Version.Parse(archiverUpgradeAttribute.Version);

                                // 判断存档版本是否升级代码
                                if (_archiver.Version < version)
                                {
                                    // 获取升级方法
                                    var upgradeMethod = deserializeObjectType.GetMethod(archiverUpgradeAttribute.UpgradeFunctionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                                    // 调用升级函数
                                    deserializeObject = upgradeMethod.Invoke(null,
                                                                             new[]
                                                                             {
                                                                                 deserializeObject
                                                                             });

                                    // 升级版本号
                                    if (upgradeVersion == null || upgradeVersion < version)
                                    {
                                        upgradeVersion = new Version(version.ToString());
                                    }

                                    if (deserializeObject == null)
                                    {
                                        throw new Exception($"\"{deserializeObjectType.FullName}.{archiverUpgradeAttribute.UpgradeFunctionName}\"返回值为null.");
                                    }
                                }
                            }

                            // 保存实体
                            _archiver.Add(deserializeObject);
                            // 读取下一个
                            reader.Read();
                        }

                        break;
                }
            }

            if (upgradeVersion != null)
            {
                _archiver.Version = upgradeVersion;
            }

            ProcessStatus = PROCESS_STATUS.NONE;
        }

        /// <summary>
        /// 收集存档类数据为Json, 并保存到指定路径中
        /// </summary>
        /// <param name="_archiver">存档</param>
        /// <param name="_savePath">存档路径</param>
        private static async UniTask CollectionData(IArchiver _archiver, string _savePath)
        {
            var fileInfo = new FileInfo(_savePath);
            if (fileInfo == null || fileInfo.Directory == null || !fileInfo.Directory.Exists)
            {
                throw new Exception($"保存路径不存在\n\"{_savePath}\"");
            }

            ProcessStatus = PROCESS_STATUS.SAVING;
            using (var jsonTextWriter = new JsonTextWriter(File.CreateText(_savePath)))
            {
                // 开始写入json
                jsonTextWriter.WriteStartObject();
                // 保存版本号
                jsonTextWriter.WritePropertyName(nameof(IArchiver.Version));
                var version = _archiver.GetType().GetProperty(nameof(IArchiver.Version)).GetValue(_archiver);
                jsonTextWriter.WriteRawValue(JsonConvert.SerializeObject(version, Formatting.None));

                // 遍历写入所有字段
                foreach (var fieldInfo in _archiver.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    jsonTextWriter.WritePropertyName(fieldInfo.Name);
                    var fieldValue = fieldInfo.GetValue(_archiver);

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

                jsonTextWriter.WriteEndObject();
            }

            ProcessStatus = PROCESS_STATUS.NONE;
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

        /// <summary>
        /// 处理状态
        /// </summary>
        public enum PROCESS_STATUS
        {
            /// <summary>
            /// 无
            /// </summary>
            NONE,

            /// <summary>
            /// 保存中
            /// </summary>
            SAVING,

            /// <summary>
            /// 加载中
            /// </summary>
            LOADING,
        }
    }
}