// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-25-17:12

using Newtonsoft.Json;

namespace EFAS.Archiver.Example
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    [Archiver("Player")]
    public class Player
    {
        /// <summary>
        /// 需要保存的数据
        /// </summary>
        public int Hp;

        /// <summary>
        /// 不需要保存的数据添加[JsonIgnore]或[NonSerialized]
        /// </summary>
        [JsonIgnore]
        // [NonSerialized]
        public bool IsDied;
    }
}