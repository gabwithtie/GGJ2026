using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

namespace GabUnity
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private int NextScene = 0;
        [SerializeField] private float delay = 0;
        [SerializeField] private UnityEvent doBeforeLoad;

        private bool already_loading = false;

        /// <summary>
        /// Reloads the current active scene immediately.
        /// </summary>
        public void ReloadScene()
        {
            if (already_loading)
                return;

            already_loading = true;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //
        }

        /// <summary>
        /// Public method to start the loading process with the configured delay and events.
        /// </summary>
        public void LoadNext()
        {
            if (already_loading)
                return;

            already_loading = true;

            StartCoroutine(LoadNextRoutine()); //
        }

        private IEnumerator LoadNextRoutine()
        {
            // 1. Invoke the UnityEvent (useful for starting fade-outs or playing sounds)
            doBeforeLoad?.Invoke(); //

            // 2. Wait for the specified delay in seconds
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay); //
            }

            // 3. Load the designated scene
            SceneManager.LoadScene(NextScene); //
        }
    }
}