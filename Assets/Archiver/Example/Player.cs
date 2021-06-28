// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-17:12

namespace EFAS.Archiver.Example
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    [ArchiverContent(typeof(ExampleArchiver), "Player")]
    public class Player
    {
        /// <summary>
        /// 需要保存的数据
        /// </summary>
        [ArchiverElement]
        public int Hp;

        /// <summary>
        /// 不需要保存的数据添加不添加[ArchiverElement]
        /// </summary>
        public bool IsDied;
    }
}