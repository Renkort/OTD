using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // one for all enemies
        [SerializeField] private float spawnRandomRange = 0f;
        [SerializeField] private bool isRandomPosition = false;
        [SerializeField] private bool spawnOnAwake = false;
        [SerializeField] private EnemySpawnData[] enemiesToSpawn;

        void Start()
        {
            if (spawnOnAwake)
                SpawnAll();
        }

        private Enemy Spawn(EnemyConfig config, Transform point)
        {
            GameObject enemyGO;
            if (isRandomPosition)
            {
                enemyGO = Instantiate(enemyPrefab, Random.insideUnitSphere * spawnRandomRange + transform.position, Quaternion.identity);
            }
            else
            {
                enemyGO = Instantiate(enemyPrefab, point.position, point.rotation);
            }
            string enemyName = config.modelPrefab.name;
            enemyGO.name = enemyName.Remove(enemyName.IndexOf("Model"));
            enemyGO.transform.SetParent(gameObject.transform);
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.Initialize(config);

            return enemy;
        }

        public List<Enemy> SpawnAll()
        {
            List<Enemy> spawnedEnemies = new();
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                for (int j = 0; j < enemiesToSpawn[i].enemyConfig.Value; j++)
                {
                    spawnedEnemies.Add(Spawn(enemiesToSpawn[i].enemyConfig.Key, enemiesToSpawn[i].point)); 
                }
            }
            return spawnedEnemies;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRandomRange);
        }
    }

    [System.Serializable]
    public struct EnemySpawnData
    {
        public Utils.KeyValuePair<EnemyConfig, int> enemyConfig;
        public Transform point;
    }
}
