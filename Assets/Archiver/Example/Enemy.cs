// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-17:13

using System.Collections.Generic;

namespace EFAS.Archiver.Example
{
    /// <summary>
    /// 敌人数据
    /// </summary>
    [ArchiverContent(typeof(ExampleArchiver), "Enemies")]
    // 升级Atk为Atk类型
    [ArchiverUpgrade("0.0.0.1", nameof(Upgrade0001))]
    // 可以添加多个升级Attribute, 注意: 版本号必须从小到大排列
    // [ArchiverUpgrade("0.0.0.2", nameof(Upgrade0002))]
    public class Enemy
    {
        /// <summary>
        /// 升级前的数据
        ///
        /// 数据只能升级不能删除
        /// </summary>
        // [ArchiverElement]
        public int Atk;

        /// <summary>
        /// 升级后的数据
        /// 0.0.0.1升级数据
        /// </summary>
        [ArchiverElement]
        public AtkInfo AtkInfo;

        /// <summary>
        /// 0.0.0.1升级函数
        /// </summary>
        /// <param name="_source">旧的存档信息</param>
        /// <returns></returns>
        private static Enemy Upgrade0001(Enemy _source)
        {
            return new Enemy()
            {
                // 为新的存档数据复制
                AtkInfo = new AtkInfo()
                {
                    AtkSet = new List<int>()
                    {
                        // 读取老的数据
                        _source.Atk
                    },
                    // 还可以添加其他的数据一同初始化
                    OnePunchKill = false,
                }
            };
        }
    }

    /// <summary>
    /// 升级后的数据类型
    /// </summary>
    public struct AtkInfo
    {
        public List<int> AtkSet;
        public bool      OnePunchKill;
    }
}