using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public bool isPaused;

    public static PauseManager Instance;

    PlayerMovement localMovement;
    CharacterAiming localAiming;
    RaycastWeapon localWeapon;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pausePanel.SetActive(false);
    }

    public void Pause()
    {
        // Do not allow pause after game over
        if (GameManager.isGameOver)
            return;

        isPaused = true;

        pausePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetLocalPlayerActive(false);
    }

    public void Resume()
    {
        // Do not resume if game already ended
        if (GameManager.isGameOver)
            return;

        isPaused = false;

        pausePanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetLocalPlayerActive(true);
    }

    public void LeaveRoom()
    {
        PhotonLauncher.Instance.LeaveRoom();
    }

    // Called from PlayerMovement Start()
    public void RegisterLocalPlayer(PlayerMovement player)
    {
        localMovement = player;
        localAiming = player.GetComponent<CharacterAiming>();
        localWeapon = player.GetComponentInChildren<RaycastWeapon>();
    }

    void SetLocalPlayerActive(bool value)
    {
        if (localMovement != null)
            localMovement.enabled = value;

        if (localAiming != null)
            localAiming.enabled = value;

        if (localWeapon != null)
            localWeapon.enabled = value;
    }

    // Called by GameManager when match ends
    public void DisablePlayerControls()
    {
        SetLocalPlayerActive(false);
    }
}