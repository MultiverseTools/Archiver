// Copyright (c) 2015 Multiverse
// Author:      Sora
// CreateTime:  2021-06-19-15:31

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using EFAS.Archiver;
using EFAS.Archiver.Example;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        private void Awake()
        {
            // Debug.Log(JsonConvert.SerializeObject(new Info()
            // {
            // Value = 888,
            // }));
        }

        public int batchCount = 10;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ListRootAsync().Forget();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("Add");

                var playerInfo = new PlayerInfo()
                {
                    Hp      = Random.Range(0, 10000),
                    Trigger = false,
                    Enemies = new List<Enemy>()
                    {
                        new Enemy()
                        {
                            Atk      = Random.Range(0, 10000),
                            Distance = Random.Range(0, 10000),
                        },
                    },
                    Counter = Random.Range(0, 10000),
                };

                ArchiverManager.AddArchiver(playerInfo);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Save");
                ArchiverManager.Save();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                var archiver = ArchiverManager.Load();
                ArchiverManager.Save(archiver, $"{Application.dataPath}/Test.json");
            }
        }


        private async UniTaskVoid ListRootAsync()
        {
            var listRoot = new List<Root>();

            var index = 0;

            for (index = 0; index < 1; index++)
            {
                var r = new Root();

                for (int i = 0; i < 2; i++)
                {
                    r.Value.Add(i);
                }

                listRoot.Add(r);
            }

            var path = Path.Combine(Application.dataPath, "Json.json");

            index = 0;
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using (var jsonTextWriter = new JsonTextWriter(sw))
            {
                jsonTextWriter.WriteStartObject();

                jsonTextWriter.WritePropertyName("ListRoot");
                jsonTextWriter.WriteStartArray();

                foreach (var root in listRoot)
                {
                    var rootJson = JsonConvert.SerializeObject(root, Formatting.Indented);
                    jsonTextWriter.WriteRawValue(rootJson);

                    if (++index % batchCount == 0)
                    {
                        index = 0;
                        await UniTask.WaitForEndOfFrame();
                    }
                }

                jsonTextWriter.WriteEndArray();

                jsonTextWriter.WritePropertyName("ListRoot2");
                jsonTextWriter.WriteStartArray();

                foreach (var root in listRoot)
                {
                    var rootJson = JsonConvert.SerializeObject(root, Formatting.Indented);
                    jsonTextWriter.WriteRawValue(rootJson);

                    if (++index % batchCount == 0)
                    {
                        index = 0;
                        await UniTask.WaitForEndOfFrame();
                    }
                }

                jsonTextWriter.WriteEndArray();

                jsonTextWriter.WriteEndObject();
            }

            // Profiler.BeginSample("WriteListRoot");
            if (File.Exists(path)) File.Delete(path);

            using (var stream = new FileStream(path, FileMode.CreateNew))
            {
                var rootJsonByteArray = Encoding.Default.GetBytes(sb.ToString());
                await stream.WriteAsync(rootJsonByteArray, 0, rootJsonByteArray.Length);
            }
            // Profiler.EndSample();

            // Profiler.BeginSample("ReadListRoot");
            var json = File.ReadAllText(path);

            var reader = new JsonTextReader(new StringReader(json));

            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    // Debug.LogFormat("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                }
                else
                {
                    // Debug.LogFormat("Token: {0}", reader.TokenType);
                }
            }
            // Profiler.EndSample();
        }
    }

    public class Root
    {
        public List<int> Value = new List<int>();
    }

    [Archiver("Info")]
    public class Info
    {
        public int Value;
    }
}