using UnityEngine;


// Enemy renderer component
namespace Akkerman.AI
{
    public class EnemyModel : MonoBehaviour
    {
        public Transform attackPoint;

        [Header("TURRET")]
        public LineRenderer lineRenderer;
        public Transform head;
        public Transform body;

    }
}
