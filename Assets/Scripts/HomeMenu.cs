using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class HomeMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;

    private const string PlayerNameKey = "PlayerName";

    void Start()
    {
        // Load saved name if exists
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            string savedName = PlayerPrefs.GetString(PlayerNameKey);
            nameInput.text = savedName;
        }
    }

    public void PlayGame()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            Debug.Log("Please enter a name");
            return;
        }

        string playerName = nameInput.text;

        // Save locally
        PlayerPrefs.SetString(PlayerNameKey, playerName);

        // Set Photon nickname
        PhotonNetwork.NickName = playerName;

        Debug.Log("Nickname set to: " + PhotonNetwork.NickName);

        // Load next scene
        SceneManager.LoadScene("MultiplayerScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}