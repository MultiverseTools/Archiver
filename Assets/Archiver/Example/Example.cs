// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:31

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

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
            if (Input.GetKeyDown(KeyCode.A))
            {
                Add();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        private void Add()
        {
            Debug.Log("Add");

            var playerInfo = new PlayerInfo()
            {
                Hp      = Random.Range(0, 10000),
                Trigger = Random.Range(0, 2) == 1,
                Enemies = new List<Enemy>()
                {
                    new Enemy()
                    {
                        Atk      = Random.Range(0, 10000),
                        Distance = Random.Range(0, 10000),
                    },
                },
            };

            ArchiverManager.AddArchiver(playerInfo);
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

        private void Load()
        {
            Debug.Log("Load");
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