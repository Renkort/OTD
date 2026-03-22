using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.Utils
{
    public class EventTimer : MonoBehaviour
    {
        [SerializeField] private float timeForEvent;
        [SerializeField] private int countBeforeStop = -1; // count < 0: infinite
        public UnityEvent OnTimeAction;
        private float lastTime = 0.0f;


        void Update()
        {
            if (countBeforeStop == 0) return;
            WaitForEvent();
        }

        private void WaitForEvent()
        {
            if (Time.time > lastTime + timeForEvent)
            {
                lastTime = Time.time;
                OnTimeAction?.Invoke();
                countBeforeStop--;
            }
        }
    }
}
