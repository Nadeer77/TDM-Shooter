using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MultiplayerScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // for editor testing
    }
}