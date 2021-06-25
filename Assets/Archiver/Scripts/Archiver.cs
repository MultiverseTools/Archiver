using System;
using System.Collections.Generic;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容
    /// </summary>
    public class Archiver
    {
        /// <summary>
        /// 存档数据类型字典
        /// </summary>
        public static Dictionary<string, Type> s_archiverDataTypeMap = new Dictionary<string, Type>()
        {

        };

        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version = new Version(0, 0, 0, 0);


        
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="_object">存档元素</param>
        public void Add(object _object)
        {
            switch (_object)
            {

            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="_object">存档元素</param>
        public void Remove(object _object)
        {
            switch (_object)
            {

            }
        }
    }
}