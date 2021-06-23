// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:08

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
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
        /// 版本号名称
        /// </summary>
        public const string k_versionName = "Version";

        /// <summary>
        /// 版本号
        /// </summary>
        public const uint k_version = 0;

        /// <summary>
        /// 每次处理的保存数据的数量
        /// </summary>
        private const int k_batchCount = 10;

        private static int s_batchIndex = 0;

        /// <summary>
        /// 存档内容
        /// </summary>
        private static Archiver s_archiver = new Archiver();

        /// <summary>
        /// 添加存档目标
        /// </summary>
        /// <param name="_object">存档</param>
        public static void AddArchiver(object _object) { }

        /// <summary>
        /// 移除存档目标
        /// </summary>
        /// <param name="_object">存档</param>
        public static void RemoveArchiver(object _object) { }

        /// <summary>
        /// 保存存档
        /// </summary>
        public static void Save()
        {
            _Save().Forget();

            async UniTaskVoid _Save()
            {
                var sb = new StringBuilder();
                await CollectionData(sb);    
            }
            
        }

        private static async UniTask CollectionData(StringBuilder _stringBuilder)
        {
            using (var stringWriter = new StringWriter(_stringBuilder))
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.WriteStartObject();

                // 写入版本号
                jsonTextWriter.WritePropertyName(k_versionName);
                jsonTextWriter.WriteValue(k_version);

                // 写入存档信息
                // 获取所有可用的字段
                foreach (var fieldInfo in typeof(Archiver).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    s_batchIndex = 0;
                    // 当前只有List类型数据
                    jsonTextWriter.WriteStartArray();
                    var set = (IEnumerable) fieldInfo.GetValue(s_archiver);

                    foreach (var item in set)
                    {
                        // TODO sora: 正式时改为 Formatting.None
                        jsonTextWriter.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented));
                        await CheckWait();
                    }

                    jsonTextWriter.WriteEndArray();
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

            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public static void Load()
        {
            // 读取文件
            // 通过版本号升级存档
        }
    }
}