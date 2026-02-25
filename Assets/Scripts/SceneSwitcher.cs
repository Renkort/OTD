using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Akkerman.FPS;


namespace Akkerman.Utils
{
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private Animator fadeAnimator;
        [SerializeField] private Player player;


        private IEnumerator FadeAndLoad(string sceneToLoad)
        {
            player.FreezePlayerActions(true, true);
            fadeAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(2f);
            player.FreezePlayerActions(false, false);
            SceneManager.LoadScene(sceneToLoad);
        }

        public void LoadScene(string sceneToLoad)
        {
            StopAllCoroutines();
            StartCoroutine(FadeAndLoad(sceneToLoad));
        }
    }
}
