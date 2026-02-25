using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace Akkerman.InteractionSystem
{
    public class CodeDoor : MonoBehaviour
    {
        [SerializeField] private string password;
        [SerializeField] private bool isLocked = true;
        [SerializeField] private float moveTime = 3.0f;
        [SerializeField] private Material redMaterial;
        [SerializeField] private UnityEvent onIncorrectPassword, onCorrectPassword;
        [SerializeField] private UnityEvent onOpenDoor, onCloseDoor;
        private string currentInput;
        private bool isOpen;

        void Start()
        {
            currentInput = "";
            if (!isLocked && !isOpen)
                TriggerDoor(true);
        }


        public void EnterSign(string sign)
        {
            currentInput += sign[0];
            if (currentInput.Length == password.Length)
            {
                bool isCorrect = currentInput == password;
                isLocked = !isCorrect;
                if (isCorrect)
                {
                    onCorrectPassword?.Invoke();
                    TriggerDoor(true);
                    Debug.Log("DEBUG: OPEN DOOR");
                }
                else
                {
                    onIncorrectPassword?.Invoke();
                    Debug.Log("DEBUG: INCORRECT PASSWORD");
                }
                currentInput = "";
            }
        }
        public void TriggerDoor(bool isOpen)
        {
            StartCoroutine(EffectDoor(isOpen));
        }

        private IEnumerator EffectDoor(bool isOpen)
        {
            Vector3 moveDirection = isOpen ? Vector3.up : Vector3.down;
            float elapsed = moveTime;
            Vector3 targetPos = transform.position + new Vector3(0.0f, transform.localScale.y, 0.0f) * moveDirection.y;
            float moveDistance = Mathf.Abs(transform.position.y - targetPos.y);
            float coef = moveDistance / moveTime;
            
            while(elapsed > 0)
            {
                transform.position += coef * Time.deltaTime * moveDirection;
                elapsed -= Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            this.isOpen = isOpen;
            if (isOpen)
                onOpenDoor?.Invoke();
            else
                onCloseDoor?.Invoke();
        }

        public void BlinkRedMaterial(GameObject gameObject)
        {
            StartCoroutine(BlinkRed(gameObject));
        }

        private IEnumerator BlinkRed(GameObject gameObject)
        {
            const int blinkTimes = 3;
            Material defaultMat = gameObject.GetComponent<MeshRenderer>().material;
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
              
            for (int i = 0; i < blinkTimes; i++)
            {
                meshRenderer.material = redMaterial;
                yield return new WaitForSeconds(0.5f);
                meshRenderer.material = defaultMat;
                yield return new WaitForSeconds(0.5f);
            }
            meshRenderer.material = defaultMat;
        }
    }
    
}
