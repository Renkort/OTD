using UnityEngine;


namespace Akkerman.AI
{
    public interface ICombat
    {
        bool CanAttack();
        void Attack(Vector3 targetPosition);
        void OnAnimEvent(string phase); // Start, Hit, End 
        float GetRange();

    }
}
