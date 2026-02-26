using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject mainPanel;

    [Header("Popup Panels")]
    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;

    void Start()
    {
        mainPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    public void OpenCreatePanel()
    {
        createRoomPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
    }

    public void OpenJoinPanel()
    {
        joinRoomPanel.SetActive(true);
        createRoomPanel.SetActive(false);
    }

    public void CloseAllPopups()
    {
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }
    public void GoBackToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
}