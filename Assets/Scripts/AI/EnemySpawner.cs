using UnityEngine;

namespace Akkerman.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // one for all enemies
        [SerializeField] private float spawnRandomRange = 0f;
        [SerializeField] private EnemySpawnData[] enemiesToSpawn;

        void Start()
        {
            SpawnAll();
        }

        private GameObject Spawn(EnemyConfig config, Transform point)
        {
            // GameObject enemyGO = Instantiate(enemyPrefab, Random.insideUnitSphere * spawnRandomRange + transform.position, Quaternion.identity);
            GameObject enemyGO = Instantiate(enemyPrefab, point.position, point.rotation);
            string enemyName = config.modelPrefab.name;
            enemyGO.name = enemyName.Remove(enemyName.IndexOf("Model"));
            enemyGO.transform.SetParent(gameObject.transform);
            enemyGO.GetComponent<Enemy>().Initialize(config);

            return enemyGO;
        }

        private void SpawnAll()
        {
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                for (int j = 0; j < enemiesToSpawn[i].enemyConfig.Value; j++)
                {
                   Spawn(enemiesToSpawn[i].enemyConfig.Key, enemiesToSpawn[i].point); 
                }
            }
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
