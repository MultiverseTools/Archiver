// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-15:38

using UnityEngine;

namespace EFAS.Archiver
{
    /// <summary>
    /// 资源工具类
    /// </summary>
    public static class ResourcesUtils
    {
        public static string TemplateContent(string _fileName) => Resources.Load<TextAsset>($"Archiver/Template/{_fileName}").text;
        
        /// <summary>
        /// 获取脚本模版
        /// </summary>
        public  static string ClassTemplateContent => TemplateContent("ClassTemplate");
    }
}