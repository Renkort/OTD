using UnityEngine;

namespace Akkerman.AI
{
    
    public interface IMovement
    {
        void MoveTo(Vector3 target);
        void Jump(Vector3 direction);
    }
}
