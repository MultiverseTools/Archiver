// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-13:50

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        public readonly string ScriptPath;

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
        /// 脚本内容
        /// </summary>
        private string m_scriptContent;

        /// <summary>
        /// 创建脚本
        /// </summary>
        /// <param name="_scriptPath">保存路径</param>
        public ScriptContent(string _scriptPath)
        {
            ScriptPath = _scriptPath;

            m_archiverDataTypeBuilder   = new StringBuilder();
            m_archiverDataSetBuilder    = new StringBuilder();
            m_archiverDataAddBuilder    = new StringBuilder();
            m_archiverDataRemoveBuilder = new StringBuilder();
            m_scriptContent             = ResourcesUtils.ClassTemplateContent;
        }

        /// <summary>
        /// 为FieldTypeMap添加元素
        /// </summary>
        /// <param name="_archiverData">存档数据</param>
        public void AppendFieldTypeIntoMap(ArchiverData _archiverData)
        {
            // "GROUP_NAME", typeof(TYPE) 
            var content = $"\"{_archiverData.GroupName}\", typeof({_archiverData.Type.FullName})";
            // {"GROUP_NAME", typeof(TYPE)} 
            m_archiverDataTypeBuilder.AppendLine("{" + content + "},");
        }

        /// <summary>
        /// 添加存档容器
        /// </summary>
        /// <param name="_archiverData">存档数据</param>
        public void AppendArchiverDataSet(ArchiverData _archiverData)
        {
            // public List<TYPE_NAME> GROUP_NAME = new List<TYPE_NAME>();
            var content = $"public List<{_archiverData.Type.FullName}> {_archiverData.GroupName} = new List<{_archiverData.Type.FullName}>();";
            m_archiverDataSetBuilder.AppendLine(content);
        }

        /// <summary>
        /// 添加存档数据
        /// </summary>
        /// <param name="_archiverData"></param>
        public void AppendAddArchiverData(ArchiverData _archiverData)
        {
            var paramName = _archiverData.Type.Name.FirstCharToLowerCase();
            // case TYPE_NAME tYPE_NAME:
            m_archiverDataAddBuilder.AppendLine($"case {_archiverData.Type.FullName} {paramName}:");
            //      GROUP_NAME.Add(tYPE_NAME);
            m_archiverDataAddBuilder.AppendLine($"\t{_archiverData.GroupName}.Add({paramName});");
            //      break;
            m_archiverDataAddBuilder.AppendLine($"\tbreak;");
            m_archiverDataAddBuilder.AppendLine();
        }

        /// <summary>
        /// 移除存档数据
        /// </summary>
        /// <param name="_archiverData"></param>
        public void AppendRemoveArchiverData(ArchiverData _archiverData)
        {
            var paramName = _archiverData.Type.Name.FirstCharToLowerCase();
            // case TYPE_NAME tYPE_NAME:
            m_archiverDataRemoveBuilder.AppendLine($"case {_archiverData.Type.FullName} {paramName}:");
            //      GROUP_NAME.Remove(tYPE_NAME);
            m_archiverDataRemoveBuilder.AppendLine($"\t{_archiverData.GroupName}.Remove({paramName});");
            //      break;
            m_archiverDataRemoveBuilder.AppendLine($"\tbreak;");
            m_archiverDataRemoveBuilder.AppendLine();
        }

        /// <summary>
        /// 完成脚本
        /// </summary>
        public void CompleteScript()
        {
            /*
             * 存档数据类型字典
             */
            var regex = new Regex(ConstantValue.k_archiverDataTypeMap);
            var match = regex.Match(m_scriptContent);
            m_scriptContent = m_scriptContent.Replace(ConstantValue.k_archiverDataTypeMap, string.Empty);
            m_scriptContent = m_scriptContent.Insert(match.Index, m_archiverDataTypeBuilder.ToString().Trim().AddTabForContent(3));
            /*
             * 存档数据集合
             */
            regex           = new Regex(ConstantValue.k_archiverDataSet);
            match           = regex.Match(m_scriptContent);
            m_scriptContent = m_scriptContent.Replace(ConstantValue.k_archiverDataSet, string.Empty);
            m_scriptContent = m_scriptContent.Insert(match.Index, m_archiverDataSetBuilder.ToString().Trim().AddTabForContent(2));
            /*
             * 添加存档数据
             */
            regex           = new Regex(ConstantValue.k_archiverDataAdd);
            match           = regex.Match(m_scriptContent);
            m_scriptContent = m_scriptContent.Replace(ConstantValue.k_archiverDataAdd, string.Empty);
            m_scriptContent = m_scriptContent.Insert(match.Index, m_archiverDataAddBuilder.ToString().Trim().AddTabForContent(4));
            /*
             * 移除存档数据
             */
            regex           = new Regex(ConstantValue.k_archiverDataRemove);
            match           = regex.Match(m_scriptContent);
            m_scriptContent = m_scriptContent.Replace(ConstantValue.k_archiverDataRemove, string.Empty);
            m_scriptContent = m_scriptContent.Insert(match.Index, m_archiverDataRemoveBuilder.ToString().Trim().AddTabForContent(4));

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
    }
}