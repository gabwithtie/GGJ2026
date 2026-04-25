using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorSceneSwitcher : MonoBehaviour
{
    [SerializeField] private bool can_switch = false;   
    [SerializeField] private int to_switch = 0;   

    // Update is called once per frame
    void Update()
    {
        if(can_switch)
            SceneManager.LoadScene(to_switch);
    }
}
