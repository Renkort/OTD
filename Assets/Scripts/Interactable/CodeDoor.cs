using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Akkerman.InteractionSystem
{
    public class CodeDoor : MonoBehaviour
    {
        private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
        [SerializeField] private string password;
        [SerializeField] private bool isLocked = true;
        [SerializeField] private float moveTime = 3.0f;
        [SerializeField] private Material redMaterial;
        [SerializeField] private List<GameObject> indicators;
        [SerializeField] private UnityEvent onIncorrectPassword, onCorrectPassword;
        [SerializeField] private UnityEvent onOpenDoor, onCloseDoor;
        private string currentInput;
        private bool isOpen;
        private bool isMoving, isBlinking;

        void Start()
        {
            currentInput = "";
            if (!isLocked && !isOpen)
                TriggerDoor(true);
            onIncorrectPassword.AddListener(BlinkRedMaterial);
        }


        public void EnterSign(string sign)
        {
            if (isBlinking || isMoving)
                return;

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
            if (this.isOpen == isOpen || isMoving)
                return;
            StartCoroutine(MoveDoor(isOpen));
        }

        private IEnumerator MoveDoor(bool isOpen)
        {
            isMoving = true;
            Vector3 moveDirection = isOpen ? Vector3.up : Vector3.down;
            float elapsed = moveTime;
            Vector3 targetPos = transform.localPosition + new Vector3(0.0f, transform.localScale.y, 0.0f) * moveDirection.y;
            float moveDistance = Mathf.Abs(transform.localPosition.y - targetPos.y);
            float coef = moveDistance / moveTime;
            
            while(elapsed > 0)
            {
                transform.localPosition += coef * Time.deltaTime * moveDirection;
                elapsed -= Time.deltaTime;
                yield return null;
            }

            transform.localPosition = targetPos;
            this.isOpen = isOpen;
            if (isOpen)
                onOpenDoor?.Invoke();
            else
                onCloseDoor?.Invoke();
            isMoving = false;
        }

        public void BlinkRedMaterial()
        {
            if (!isBlinking)
                StartCoroutine(BlinkRed());
        }

        private IEnumerator BlinkRed()
        {
            isBlinking = true;
            const int blinkTimes = 3;
            List<MeshRenderer> meshRenderers = new();
            List<Material> defaultMat = new();
            for (int j = 0; j < indicators.Count; j++)
            {
                meshRenderers.Add(indicators[j].GetComponent<MeshRenderer>());
                defaultMat.Add(meshRenderers[j].material);
            }
            for (int i = 0; i < blinkTimes; i++)
            {
                for (int j =0; j < indicators.Count; j++)
                    meshRenderers[j].material = redMaterial;
                yield return _waitForSeconds0_5;

                for (int j =0; j < indicators.Count; j++)
                    meshRenderers[j].material = defaultMat[j];
                yield return _waitForSeconds0_5;
            }
            for (int i=0; i < meshRenderers.Count; i++)
                meshRenderers[i].material = defaultMat[i];
            isBlinking = false;
        }
    }
    
}
