// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-16:19

using System;
using System.Text;

namespace EFAS.Archiver
{
    public static class EditorUtils
    {
        /// <summary>
        /// 首字母设置为小写
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static string FirstCharToLowerCase(this string _str)
        {
            if (string.IsNullOrEmpty(_str) || char.IsLower(_str[0]))
                return _str;

            return char.ToLower(_str[0]) + _str.Substring(1);
        }
        
        /// <summary>
        /// 给内容块添加制表符
        /// </summary>
        /// <param name="_content">内容</param>
        /// <param name="_tabCount">制表符数量</param>
        /// <returns></returns>
        public static string AddTabForContent(this string _content, int _tabCount)
        {
            var sb = new StringBuilder();

            var allLines = _content.Split(new string[]
                {
                    Environment.NewLine
                },
                StringSplitOptions.None);

            var tabString = string.Empty;

            for (var index = 0; index < _tabCount; index++)
            {
                tabString += "\t";
            }

            foreach (var line in allLines)
            {
                sb.AppendLine($"{tabString}{line}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}