using Unity.VisualScripting;
using UnityEngine;

namespace Akkerman.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // one for all enemies
        [SerializeField] private EnemyConfig[] allEnemyConfigs;
        [SerializeField] private float spawnRandomRange = 0f;
        [SerializeField] private Utils.KeyValuePair<EnemyConfig, int>[] enemiesToSpawn;

        void Start()
        {
            TestSpawn();
        }

        public void Spawn(string configName)
        {
            EnemyConfig config = System.Array.Find(allEnemyConfigs, c => c.name == configName);
            if (config == null) return;

            GameObject enemyGO = Instantiate(enemyPrefab, Random.insideUnitSphere * spawnRandomRange + transform.position, Quaternion.identity);
            enemyGO.transform.SetParent(gameObject.transform);
            enemyGO.GetComponent<Enemy>().Initialize(config);
        }

        private void TestSpawn()
        {
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                for (int j = 0; j < enemiesToSpawn[i].Value; j++)
                {
                   Spawn(enemiesToSpawn[i].Key.name); 
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRandomRange);
        }
    }
}
