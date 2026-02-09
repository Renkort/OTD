using UnityEngine;
using UnityEngine.Events;


namespace Akkerman.InteractionSystem
{
    public class CodeDoor : MonoBehaviour
    {
        [SerializeField] private string password;
        [SerializeField] private bool isLocked = true;
        [SerializeField] private UnityEvent onIncorrectPassword, onCorrectPassword;
        private string currentInput;

        public void EnterSign(string sign)
        {
            currentInput += sign;
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
            Vector3 movePos = gameObject.transform.position - gameObject.transform.localScale;
            gameObject.transform.position = movePos;
        }
    }
    
}
