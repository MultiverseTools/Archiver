// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:31

using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EFAS.Archiver.Example
{
    /// <summary>
    /// 示例
    /// </summary>
    public class Example : MonoBehaviour
    {
        private ExampleArchiver m_exampleArchiver = new ExampleArchiver();
        private string          m_archiverPath;
        private string          m_upgradeArchiverPath;

        private void Awake()
        {
            m_archiverPath        = $"{Application.dataPath}/com.multiversetools.archiver/Example/Archiver.json";
            m_upgradeArchiverPath = $"{Application.dataPath}/com.multiversetools.archiver/Example/UpgradeArchiver.json";
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                AddPlayer();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                AddEnemy();
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadAndUpgrade();
            }
        }

        private Player m_player; 

        /// <summary>
        /// 添加一个玩家
        /// </summary>
        private void AddPlayer()
        {
            m_player = new Player()
            {
                Hp = Random.Range(0, 100000),
                Atk = new Atk()
                {
                    Value = Random.Range(0, 10000),
                }
            };

            Debug.Log($"AddPlayer: {m_player.Atk.Value}");
            // 添加存档目标
            m_exampleArchiver.Add(m_player);
        }

        /// <summary>
        /// 添加一个敌人
        /// </summary>
        private void AddEnemy()
        {
            Debug.Log("AddEnemy");

            var enemy = new Enemy()
            {
                Atk = Random.Range(0, 100000),
            };

            // 添加存档目标
            m_exampleArchiver.Add(enemy);
        }

        /// <summary>
        /// 保存存档
        /// </summary>
        private void Save()
        {
            Debug.Log("Save");
            _Save().Forget();

            async UniTaskVoid _Save()
            {
                // 保存存档
                await ArchiverManager.SaveArchiver(m_exampleArchiver, m_archiverPath);
                Debug.Log("Save Complete");
            }
        }

        /// <summary>
        /// 加载存档并升级
        /// </summary>
        private void LoadAndUpgrade()
        {
            Debug.Log("LoadAndUpgrade");
            _Load().Forget();

            async UniTaskVoid _Load()
            {
                m_exampleArchiver.Clear();
                // 从指定位置读取存档
                await ArchiverManager.LoadArchiver(m_exampleArchiver, m_archiverPath);
                Debug.Log("Load Complete");
                // 保存存档到指定位置
                await ArchiverManager.SaveArchiver(m_exampleArchiver, m_upgradeArchiverPath);
                Debug.Log("Save Complete");
            }
        }
    }
}