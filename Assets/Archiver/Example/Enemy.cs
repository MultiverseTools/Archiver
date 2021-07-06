// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-17:13

using System.Collections.Generic;
using UnityEngine;

namespace EFAS.Archiver.Example
{
    /// <summary>
    /// 敌人数据
    /// </summary>
    // [ArchiverContent(typeof(ExampleArchiver), "Enemies")]
    // 升级Atk为Atk类型
    // [ArchiverUpgrade("0.0.0.1", nameof(Upgrade0001))]
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
        /// 只保存AtkSet信息
        /// 0.0.0.1升级数据
        /// </summary>
        [ArchiverElement(nameof(Archiver.Example.AtkInfo.AtkSet))]
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
                // 嵌套升级需要上传下层升级的数据
                // 嵌套升级会先执行下层的升级(Struct AtkInfo)
                AtkInfo = _source.AtkInfo,
                Atk = _source.Atk + Random.Range(0, 1000),
            };
        }
    }

    /// <summary>
    /// 升级后的数据类型
    /// </summary>
    // 注意: 嵌套升级需要在上层也返回当前层的值比如
    [ArchiverUpgrade("0.0.0.2", nameof(Upgrade0001))]
    public struct AtkInfo
    {
        [ArchiverElement] 
        public List<int> AtkSet;

        [ArchiverElement] 
        public bool OnePunchKill;

        private static AtkInfo Upgrade0001(AtkInfo _source)
        {
            return new AtkInfo()
            {
                AtkSet = new List<int>()
                {
                    Random.Range(0, 1000),
                    Random.Range(1000, 2000),
                },
                OnePunchKill = true,
            };
        }
    }
}