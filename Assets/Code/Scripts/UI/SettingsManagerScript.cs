using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManagerScript : MonoBehaviour
{
    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
