using UnityEngine;
using UnityEngine.SceneManagement;


namespace GabUnity
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private int NextScene = 0;

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void LoadNext()
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}
