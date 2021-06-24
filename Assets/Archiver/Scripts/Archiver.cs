// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:29

using System;
using System.Collections.Generic;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容
    /// </summary>
    public class Archiver
    {
        public static Dictionary<string, Type> s_fieldTypeMap = new Dictionary<string, Type>()
        {
            {
                "ExamplePlayerInfo", typeof(EFAS.Archiver.Example.PlayerInfo)
            },
        };

        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version = new Version(0, 0, 0, 0);

        /// <summary>
        /// TODO 生成代码收集所有存档分组
        /// </summary>
        public List<EFAS.Archiver.Example.PlayerInfo> ExamplePlayerInfo = new List<EFAS.Archiver.Example.PlayerInfo>();
        
        /// <summary>
        /// TODO 生成添加物品
        /// TODO 生成类型
        /// </summary>
        /// <param name="_object"></param>
        public void Add(object _object)
        {
            switch (_object)
            {
                case EFAS.Archiver.Example.PlayerInfo playerInfo:
                    ExamplePlayerInfo.Add(playerInfo);
                    break;
            }
        }

        /// <summary>
        /// TODO 生成代码删除物品
        /// TODO 生成代码类型
        /// </summary>
        /// <param name="_object"></param>
        public void Remove(object _object)
        {
            switch (_object)
            {
                case EFAS.Archiver.Example.PlayerInfo playerInfo:
                    ExamplePlayerInfo.Remove(playerInfo);
                    break;
            }
        }
    }
}