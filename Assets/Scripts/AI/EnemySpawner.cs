using UnityEngine;

namespace Akkerman.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab; // one for all enemies
        [SerializeField] private EnemyConfig[] allEnemyConfigs;
        [SerializeField] private float spawnRandomRange = 3f;

        void Start()
        {
            Spawn("BoxerConfig"); // for tests
        }

        public void Spawn(string configName)
        {
            EnemyConfig config = System.Array.Find(allEnemyConfigs, c => c.name == configName);
            if (config == null) return;

            GameObject enemyGO = Instantiate(enemyPrefab, Random.insideUnitSphere * spawnRandomRange + transform.position, Quaternion.identity);
            enemyGO.GetComponent<Enemy>().Initialize(config);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRandomRange);
        }
    }
}
