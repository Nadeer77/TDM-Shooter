using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MultiplayerUI : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject mainPanel;

    [Header("Popup Panels")]
    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;

    [Header("Create Room Inputs")]
    public TMP_InputField createRoomIdInput;
    public TMP_InputField createPasswordInput;

    [Header("Join Room Inputs")]
    public TMP_InputField joinRoomIdInput;
    public TMP_InputField joinPasswordInput;

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

    public void OnCreateRoomButtonClicked()
    {
        string roomId = createRoomIdInput.text;
        string password = createPasswordInput.text;

        if (PhotonLauncher.Instance != null)
        {
            PhotonLauncher.Instance.CreateRoom(roomId);
        }
        else
        {
            Debug.LogError("PhotonLauncher instance not found!");
        }
    }

    public void OnJoinRoomButtonClicked()
    {
        string roomId = joinRoomIdInput.text;

        if (PhotonLauncher.Instance != null)
        {
            PhotonLauncher.Instance.JoinRoom(roomId);
        }
        else
        {
            Debug.LogError("PhotonLauncher instance not found!");
        }
    }
}