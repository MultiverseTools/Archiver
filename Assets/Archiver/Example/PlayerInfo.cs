// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-23-17:38

using System.Collections.Generic;
using Newtonsoft.Json;

namespace EFAS.Archiver.Example
{
    [Archiver("ExamplePlayerInfo")]
    public class PlayerInfo
    {
        public int Hp;

        public bool Trigger;

        public List<Enemy> Enemies;

        [JsonIgnore]
        public int Counter;
    }
}