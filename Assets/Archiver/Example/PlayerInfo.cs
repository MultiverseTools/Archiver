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
    [ArchiverUpgrade("0.0.0.2", nameof(VersionUpdate2))]
    public class PlayerInfo
    {
        // {"Version":"0.0.0.0","ExamplePlayerInfo":[{"Hp":8793,"Trigger":false,"Enemies":[{"Atk":2632,"Distance":404.0}]},{"Hp":6851,"Trigger":true,"Enemies":[{"Atk":155,"Distance":2049.0}]}]}
        // {"Version":"0.0.0.1","ExamplePlayerInfo":[{"Hp":0,"Trigger":false,"Enemies":[{"Atk":2632,"Distance":404.0}],"HpInfo":{"Hp":8793,"TestValue":3367}},{"Hp":0,"Trigger":true,"Enemies":[{"Atk":155,"Distance":2049.0}],"HpInfo":{"Hp":6851,"TestValue":971}}]}
        // {"Version":"0.0.0.2","ExamplePlayerInfo":[{"Hp":0,"Trigger":false,"Enemies":[{"Atk":2632,"Distance":404.0}],"HpInfo":{"Hp":8793,"TestValue":3367},"TriggerState":0},{"Hp":0,"Trigger":false,"Enemies":[{"Atk":155,"Distance":2049.0}],"HpInfo":{"Hp":6851,"TestValue":971},"TriggerState":6607}]}
        /// <summary>
        /// 1 版本过期
        /// </summary>
        // [NonSerialized]
        [Obsolete]
        // TODO 如何去除过期数据
        public int Hp;

        /// <summary>
        /// 2 版本过期
        /// </summary>
        // [JsonIgnore]
        [Obsolete]
        public bool Trigger;

        public List<Enemy> Enemies;

        public HpInfo HpInfo;

        // [JsonIgnore]
        public int TriggerState;

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

        public static PlayerInfo VersionUpdate2(PlayerInfo _source)
        {
            return new PlayerInfo()
            {
                Enemies      = new List<Enemy>(_source.Enemies),
                HpInfo       = _source.HpInfo,
                TriggerState = _source.Trigger ? UnityEngine.Random.Range(0, 10000) : 0,
            };
        }
    }

    public struct HpInfo
    {
        public int Hp;
        public int TestValue;
    }
}