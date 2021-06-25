// codegen

using System;
using System.Collections.Generic;

namespace EFAS.Archiver
{
    /// <summary>
    /// 存档内容
    /// </summary>
    [Archiver("Assets/Archiver/Example")]
    public class ExampleArchiver : IArchiver
    {
        /// <summary>
        /// 存档数据类型字典
        /// </summary>
        public Dictionary<string, Type> ArchiverDataTypeMap { get; } = new Dictionary<string, Type>()
        {
			{"Enemies", typeof(EFAS.Archiver.Example.Enemy)},
			{"Player", typeof(EFAS.Archiver.Example.Player)},
        };

        /// <summary>
        /// 版本号
        /// </summary>
        [Newtonsoft.Json.JsonProperty]
        public Version Version { get; set; } = new Version(0, 0, 0, 0);

		public List<EFAS.Archiver.Example.Enemy> Enemies = new List<EFAS.Archiver.Example.Enemy>();
		public List<EFAS.Archiver.Example.Player> Player = new List<EFAS.Archiver.Example.Player>();
        
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="_object">存档元素</param>
        public void Add(object _object)
        {
            switch (_object)
            {
				case EFAS.Archiver.Example.Enemy enemy:
					Enemies.Add(enemy);
					break;
				
				case EFAS.Archiver.Example.Player player:
					Player.Add(player);
					break;
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="_object">存档元素</param>
        public void Remove(object _object)
        {
            switch (_object)
            {
				case EFAS.Archiver.Example.Enemy enemy:
					Enemies.Remove(enemy);
					break;
				
				case EFAS.Archiver.Example.Player player:
					Player.Remove(player);
					break;
            }
        }
    }
}