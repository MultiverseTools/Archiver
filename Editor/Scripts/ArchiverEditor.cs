// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-12:37

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EFAS.Archiver
{
    public class ArchiverEditor
    {
        /// <summary>
        /// 生成Archiver脚本
        /// </summary>
        [MenuItem("Build/生成存档类-Archiver.cs")]
        private static void GenerateArchiver()
        {
            var archiverDataSet = new List<ArchiverData>();
            CollectionArchiverData(archiverDataSet);

            // 脚本路径
            var scriptPath = $"{Application.dataPath}/Archiver/Scripts/Archiver.cs";
            // 脚本内容
            var scriptContent = new ScriptContent(scriptPath);

            // 构建脚本内容
            foreach (var archiverData in archiverDataSet)
            {
                scriptContent.AppendFieldTypeIntoMap(archiverData);
                scriptContent.AppendArchiverDataSet(archiverData);
                scriptContent.AppendAddArchiverData(archiverData);
                scriptContent.AppendRemoveArchiverData(archiverData);
            }

            scriptContent.CompleteScript();
            // 更新Unity
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 收集存档数据
        /// </summary>
        /// <param name="_archiverData"></param>
        private static void CollectionArchiverData(List<ArchiverData> _archiverData)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var archiverAttribute = type.GetCustomAttribute<ArchiverAttribute>();
                    if (archiverAttribute == null) continue;
                    // 检测存档升级能否使用
                    type.CheckUpgradeValid();

                    _archiverData.Add(new ArchiverData()
                    {
                        GroupName = archiverAttribute.GroupName,
                        Type      = type,
                    });
                }
            }
        }
    }
}