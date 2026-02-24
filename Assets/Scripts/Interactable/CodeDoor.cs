using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace Akkerman.InteractionSystem
{
    public class CodeDoor : MonoBehaviour
    {
        [SerializeField] private string password;
        [SerializeField] private bool isLocked = true;
        [SerializeField] private Material redMaterial;
        [SerializeField] private UnityEvent onIncorrectPassword, onCorrectPassword;
        private string currentInput;
        private bool isMoving = false;
        private float moveTimer;
        private float moveTime = 3.0f;
        private int moveSpeed = 1;

        void Start()
        {
            currentInput = "";
        }


        void Update()
        {
            if (isMoving)
            {
                
                if (moveTimer <= Time.time)
                    isMoving = false;
                Vector3 moveDirection = Vector3.down * moveSpeed;
                gameObject.transform.position -= moveDirection * Time.deltaTime;
            }
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
                    OpenDoor();
                    Debug.Log("Password's correct!");
                }
                else
                {

                    onIncorrectPassword?.Invoke();
                    Debug.Log("Password is incorrect!");
                }
                currentInput = "";
            }
        }

        private void OpenDoor()
        {
            // Vector3 movePos = gameObject.transform.position - gameObject.transform.localScale;
            // gameObject.transform.position = movePos;
            moveTimer = Time.time + moveTime;
            isMoving = true;
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
