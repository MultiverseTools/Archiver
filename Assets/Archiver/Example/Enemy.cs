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
    [ArchiverElementFilter]
    // 升级Atk为Atk类型
    [ArchiverUpgrade("0.0.0.1", nameof(Upgrade0001))]
    // 可以添加多个升级Attribute, 注意: 版本号必须从小到大排列
    public class Enemy
    {
        /*
         * 升级操作
         * 1) 在Class/Struct上添加[ArchiverUpgrade]标签
         * 2) 指定版本号(注意: 版本号必须从小到大排列)
         * 3) 指定升级方法(注意: 返回值不能为null)
         * 4) 移除过期数据上[ArchiverElement]标签, 为新数据添加[ArchiverElement]标签
         */

        /// <summary>
        /// 升级前的数据
        ///
        /// 数据只能升级不能删除
        /// </summary>
        [ArchiverElement]
        public int Atk;

        /// <summary>
        /// 升级后的数据
        /// 0.0.0.1升级数据
        /// </summary>
        // [ArchiverElement] 
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
    [ArchiverElementFilter]
    public struct AtkInfo
    {
        [ArchiverElement] public List<int> AtkSet;

        public bool OnePunchKill;
    }
}