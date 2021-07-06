// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-13:50

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EFAS.Archiver
{
    /// <summary>
    /// 脚本内容
    /// </summary>
    public class ScriptContent
    {
        /// <summary>
        /// 脚本保存路径
        /// </summary>
        public string ScriptPath => $"{GenerateFolderPath}/{GenerateType.Name}.cs";

        /// <summary>
        /// 生成目录
        /// </summary>
        public readonly string GenerateFolderPath;

        /// <summary>
        /// 生成类型
        /// </summary>
        public readonly Type GenerateType;

        /// <summary>
        /// 命名空间
        /// </summary>
        public readonly string Namespace;

        /// <summary>
        /// 类名
        /// </summary>
        public readonly string ClassName;

        /// <summary>
        /// 存档数据类型字典builder
        /// </summary>
        private StringBuilder m_archiverDataTypeBuilder;

        /// <summary>
        /// 存档数据集合builder
        /// </summary>
        private StringBuilder m_archiverDataSetBuilder;

        /// <summary>
        /// 添加存档数据builder
        /// </summary>
        private StringBuilder m_archiverDataAddBuilder;

        /// <summary>
        /// 移除存档数据builder
        /// </summary>
        private StringBuilder m_archiverDataRemoveBuilder;

        /// <summary>
        /// 清空存档数据builder
        /// </summary>
        private StringBuilder m_archiverDataClearBuilder;

        /// <summary>
        /// 脚本内容
        /// </summary>
        private string m_scriptContent;

        /// <summary>
        /// 创建脚本
        /// </summary>
        /// <param name="_generateFolderPath">保存路径</param>
        /// <param name="_generateType">生成类型</param>
        /// <param name="_namespace">命名空间</param>
        /// <param name="_className">类型名称</param>
        public ScriptContent(string _generateFolderPath, Type _generateType, string _namespace, string _className)
        {
            GenerateFolderPath = _generateFolderPath;
            GenerateType       = _generateType;
            Namespace          = _namespace;
            ClassName          = _className;

            m_archiverDataTypeBuilder   = new StringBuilder();
            m_archiverDataSetBuilder    = new StringBuilder();
            m_archiverDataAddBuilder    = new StringBuilder();
            m_archiverDataRemoveBuilder = new StringBuilder();
            m_archiverDataClearBuilder  = new StringBuilder();
            m_scriptContent             = ResourcesUtils.ClassTemplateContent;
        }

        /// <summary>
        /// 为FieldTypeMap添加元素
        /// </summary>
        /// <param name="_archiverContentData">存档数据</param>
        public void AppendFieldTypeIntoMap(ArchiverContentData _archiverContentData)
        {
            // "GROUP_NAME", typeof(TYPE) 
            var content = $"\"{_archiverContentData.GroupName}\", typeof({_archiverContentData.Type.FullName})";
            // {"GROUP_NAME", typeof(TYPE)} 
            m_archiverDataTypeBuilder.AppendLine("{" + content + "},");
        }

        /// <summary>
        /// 添加存档容器
        /// </summary>
        /// <param name="_archiverContentData">存档数据</param>
        public void AppendArchiverDataSet(ArchiverContentData _archiverContentData)
        {
            // public List<TYPE_NAME> GROUP_NAME = new List<TYPE_NAME>();
            var content = $"public List<{_archiverContentData.Type.FullName}> {_archiverContentData.GroupName} = new List<{_archiverContentData.Type.FullName}>();";
            m_archiverDataSetBuilder.AppendLine(content);
        }

        /// <summary>
        /// 添加存档数据
        /// </summary>
        /// <param name="_archiverContentData"></param>
        public void AppendAddArchiverData(ArchiverContentData _archiverContentData)
        {
            var paramName = _archiverContentData.Type.Name.FirstCharToLowerCase();
            // case TYPE_NAME tYPE_NAME:
            m_archiverDataAddBuilder.AppendLine($"case {_archiverContentData.Type.FullName} {paramName}:");
            //      GROUP_NAME.Add(tYPE_NAME);
            m_archiverDataAddBuilder.AppendLine($"\t{_archiverContentData.GroupName}.Add({paramName});");
            //      break;
            m_archiverDataAddBuilder.AppendLine($"\tbreak;");
            m_archiverDataAddBuilder.AppendLine();
        }

        /// <summary>
        /// 移除存档数据
        /// </summary>
        /// <param name="_archiverContentData"></param>
        public void AppendRemoveArchiverData(ArchiverContentData _archiverContentData)
        {
            var paramName = _archiverContentData.Type.Name.FirstCharToLowerCase();
            // case TYPE_NAME tYPE_NAME:
            m_archiverDataRemoveBuilder.AppendLine($"case {_archiverContentData.Type.FullName} {paramName}:");
            //      GROUP_NAME.Remove(tYPE_NAME);
            m_archiverDataRemoveBuilder.AppendLine($"\t{_archiverContentData.GroupName}.Remove({paramName});");
            //      break;
            m_archiverDataRemoveBuilder.AppendLine($"\tbreak;");
            m_archiverDataRemoveBuilder.AppendLine();
        }

        /// <summary>
        /// 清空存档数据
        /// </summary>
        /// <param name="_archiverContentData"></param>
        public void AppendClearArchiverData(ArchiverContentData _archiverContentData)
        {
            var content = $"{_archiverContentData.GroupName}.Clear();";
            m_archiverDataClearBuilder.AppendLine(content);
        }

        /// <summary>
        /// 完成脚本
        /// </summary>
        public void CompleteScript()
        {
            // 设置存档路径
            ReplaceContent(ConstantValue.k_generateFolderPath, GenerateFolderPath);
            // 修改命名空间
            ReplaceContent(ConstantValue.k_namespace, Namespace);
            // 修改类名
            ReplaceContent(ConstantValue.k_className, ClassName);
            // 存档数据类型字典
            ReplaceContent(ConstantValue.k_archiverDataTypeMap, m_archiverDataTypeBuilder.ToString().Trim().AddTabForContent(3));
            // 存档数据集合
            ReplaceContent(ConstantValue.k_archiverDataSet, m_archiverDataSetBuilder.ToString().Trim().AddTabForContent(2));
            // 添加存档数据
            ReplaceContent(ConstantValue.k_archiverDataAdd, m_archiverDataAddBuilder.ToString().Trim().AddTabForContent(4));
            // 移除存档数据
            ReplaceContent(ConstantValue.k_archiverDataRemove, m_archiverDataRemoveBuilder.ToString().Trim().AddTabForContent(4));
            // 清空存档数据
            ReplaceContent(ConstantValue.k_archiverDataClear, m_archiverDataClearBuilder.ToString().Trim().AddTabForContent(3));

            // 删除已存在脚本
            if (File.Exists(ScriptPath))
            {
                File.Delete(ScriptPath);
            }

            var streamWriter = File.CreateText(ScriptPath);
            // 写入文件
            streamWriter.Write(m_scriptContent);
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }

        /// <summary>
        /// 替换指定key的内容
        /// </summary>
        /// <param name="_replaceKey">key</param>
        /// <param name="_replaceContent">内容</param>
        private void ReplaceContent(string _replaceKey, string _replaceContent)
        {
            var regex = new Regex(_replaceKey);
            var match = regex.Match(m_scriptContent);
            do
            {
                m_scriptContent = m_scriptContent.Remove(match.Index, _replaceKey.Length);
                m_scriptContent = m_scriptContent.Insert(match.Index, _replaceContent);
                match           = regex.Match(m_scriptContent);
            } while (match.Success);
        }
    }
}