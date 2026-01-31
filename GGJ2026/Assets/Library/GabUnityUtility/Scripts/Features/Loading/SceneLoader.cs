using UnityEngine;
using UnityEngine.SceneManagement;


namespace GabUnity
{
    public class SceneLoader : MonoBehaviour
    {

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
