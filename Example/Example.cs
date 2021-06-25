// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:31

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EFAS.Archiver.Example
{
    public class Example : MonoBehaviour
    {
        private string m_archiverPath;
        private string m_upgradeArchiverPath;

        private void Awake()
        {
            m_archiverPath        = $"{Application.dataPath}/Archiver/Example/Archiver.json";
            m_upgradeArchiverPath = $"{Application.dataPath}/Archiver/Example/UpgradeArchiver.json";
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

        private void AddPlayer()
        {
            Debug.Log("AddPlayer");

            var player = new Player()
            {
                Hp = Random.Range(0, 100000),
            };

            ArchiverManager.AddArchiver(player);
        }

        private void AddEnemy()
        {
            Debug.Log("AddEnemy");

            var enemy = new Enemy()
            {
                Atk = Random.Range(0, 100000),
            };

            ArchiverManager.AddArchiver(enemy);
        }

        private void Save()
        {
            Debug.Log("Save");
            _Save().Forget();

            async UniTaskVoid _Save()
            {
                await ArchiverManager.SaveArchiver(ArchiverManager.s_archiver, m_archiverPath);
                Debug.Log("Save Complete");
            }
        }

        private void LoadAndUpgrade()
        {
            Debug.Log("LoadAndUpgrade");
            _Load().Forget();

            async UniTaskVoid _Load()
            {
                var archiver = new Archiver();
                await ArchiverManager.LoadArchiver(archiver, m_archiverPath);
                Debug.Log("Load Complete");
                await ArchiverManager.SaveArchiver(archiver, m_upgradeArchiverPath);
                Debug.Log("Save Complete");
            }
        }
    }
}