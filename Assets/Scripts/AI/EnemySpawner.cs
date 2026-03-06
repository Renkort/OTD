using UnityEngine;

namespace Akkerman.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // one for all enemies
        [SerializeField] private float spawnRandomRange = 0f;
        [SerializeField] private Utils.KeyValuePair<EnemyConfig, int>[] enemiesToSpawn;

        void Start()
        {
            TestSpawn();
        }

        public void Spawn(EnemyConfig config)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, Random.insideUnitSphere * spawnRandomRange + transform.position, Quaternion.identity);
            string enemyName = config.modelPrefab.name;
            enemyGO.name = enemyName.Remove(enemyName.IndexOf("Model"));
            enemyGO.transform.SetParent(gameObject.transform);
            enemyGO.GetComponent<Enemy>().Initialize(config);
        }

        private void TestSpawn()
        {
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                for (int j = 0; j < enemiesToSpawn[i].Value; j++)
                {
                   Spawn(enemiesToSpawn[i].Key); 
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
