// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-23-17:38

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EFAS.Archiver.Example
{
    [Archiver("ExamplePlayerInfo")]
    [ArchiverUpgrade("0.0.0.1", nameof(VersionUpdate1))]
    // [ArchiverUpdate(2, nameof(VersionUpdate2))]
    public class PlayerInfo
    {
        /// <summary>
        /// 1 版本过期
        /// </summary>
        // [NonSerialized]
        [Obsolete]
        public int Hp;

        /// <summary>
        /// 2 版本过期
        /// </summary>
        // [JsonIgnore]
        // [Obsolete]
        public bool Trigger;

        public List<Enemy> Enemies;

        [JsonIgnore]
        public int Counter;

        public HpInfo HpInfo;

        // public int TriggerState;

        public static PlayerInfo VersionUpdate1(PlayerInfo _source)
        {
            return new PlayerInfo
            {
                Trigger = _source.Trigger,
                Enemies = new List<Enemy>(_source.Enemies),
                HpInfo = new HpInfo()
                {
                    Hp        = _source.Hp,
                    TestValue = UnityEngine.Random.Range(0, 10000),
                },
            };
        }

        // public void VersionUpdate2() { TriggerState = Trigger ? 999 : 0; }
    }

    public struct HpInfo
    {
        public int Hp;
        public int TestValue;
    }
}