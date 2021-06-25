// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-12:37

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace EFAS.Archiver
{
    public class ArchiverEditor
    {
        /// <summary>
        /// 生成Archiver脚本
        /// </summary>
        [MenuItem("Build/生成存档类")]
        private static void GenerateArchiver()
        {
            var archiverDataMap = new Dictionary<Type, ArchiverData>();
            CollectionArchiverData(archiverDataMap);

            // 构建脚本内容
            foreach (var archiverData in archiverDataMap.Values)
            {
                // 脚本内容
                var scriptContent = new ScriptContent(archiverData.GenerateFoldPath, archiverData.Type);

                foreach (var archiverContentData in archiverData.ArchiverContentSet)
                {
                    scriptContent.AppendFieldTypeIntoMap(archiverContentData);
                    scriptContent.AppendArchiverDataSet(archiverContentData);
                    scriptContent.AppendAddArchiverData(archiverContentData);
                    scriptContent.AppendRemoveArchiverData(archiverContentData);
                }

                scriptContent.CompleteScript();
            }

            // 更新Unity
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 收集存档数据
        /// </summary>
        /// <param name="_archiverDataSet"></param>
        private static void CollectionArchiverData(Dictionary<Type, ArchiverData> _archiverDataSet)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var archiverAttribute = type.GetCustomAttribute<ArchiverAttribute>();
                    if (archiverAttribute == null) continue;

                    if (!_archiverDataSet.ContainsKey(type))
                    {
                        _archiverDataSet.Add(type,
                            new ArchiverData()
                            {
                                GenerateFoldPath   = archiverAttribute.GenerateFoldPath,
                                Type               = type,
                                ArchiverContentSet = new List<ArchiverContentData>(),
                            });
                    }
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var archiverAttribute = type.GetCustomAttribute<ArchiverContentAttribute>();
                    if (archiverAttribute == null) continue;
                    // 检测存档升级能否使用
                    type.CheckUpgradeValid();

                    if (!_archiverDataSet.TryGetValue(archiverAttribute.ArchiverType, out var archiverData))
                    {
                        throw new Exception($"\"{archiverAttribute.ArchiverType.Name}\"未添加\"[{nameof(ArchiverAttribute)}]\".");
                    }

                    archiverData.ArchiverContentSet.Add(new ArchiverContentData
                    {
                        GroupName = archiverAttribute.GroupName,
                        Type      = type,
                    });
                }
            }
        }
    }
}