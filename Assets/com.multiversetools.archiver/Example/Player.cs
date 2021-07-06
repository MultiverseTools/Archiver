// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-17:12

namespace EFAS.Archiver.Example
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    // [ArchiverContent(typeof(ExampleArchiver), "Player")]
    public class Player
    {
        /// <summary>
        /// 需要保存的数据
        /// </summary>
        [ArchiverElement]
        public int Hp;

        /// <summary>
        /// 保存Class/Struct时, 如果Class/Struct内包含[ArchiverElement]标签才会执行过滤, 不然会保存整个类型数据
        /// 参考<see cref="Atk"/>
        /// </summary>
        [ArchiverElement]
        public Mp Mp;

        [ArchiverElement]
        public Atk Atk;

        /// <summary>
        /// 不需要保存的数据添加不添加[ArchiverElement]
        /// </summary>
        public bool IsDied;
    }

    public struct Mp
    {
        public int Value;

        public bool AutoRecovery;
    }

    public struct Atk
    {
        [ArchiverElement]
        // 注意: 不要使用[field: ArchiverElement], backfield是private类型不能被保存
        public int Value { get; set; }

        public bool OnePunch;
    }
}