using UnityEngine;

namespace Akkerman.AI
{
    public interface IAIBrain
    {
        void UpdateAI(Vector3 playerPosition);
        Vector3? GetTargetPosition();
        int GetCurrentState();
    }
    
}
