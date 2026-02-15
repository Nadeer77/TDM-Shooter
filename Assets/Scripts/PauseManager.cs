using UnityEngine;
using Photon.Pun;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // ---------- PAUSE ----------
    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        DisableLocalPlayer(true);
    }

    // ---------- RESUME ----------
    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        DisableLocalPlayer(false);
    }

    // ---------- LEAVE ROOM ----------
    public void LeaveRoom()
    {
        Debug.Log("Leaving Room...");
        PhotonNetwork.LeaveRoom();
    }

    // ---------- LOCAL PLAYER DISABLE ----------
    void DisableLocalPlayer(bool value)
    {
        PlayerMovement movement =
            FindFirstObjectByType<PlayerMovement>();

        if (movement != null)
            movement.enabled = !value;

        CharacterAiming aiming =
            FindFirstObjectByType<CharacterAiming>();

        if (aiming != null)
            aiming.enabled = !value;
    }
}
