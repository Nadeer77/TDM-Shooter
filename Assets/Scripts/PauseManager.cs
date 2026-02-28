using UnityEngine;
using Photon.Pun;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public bool isPaused;

    public static PauseManager Instance;
    PlayerMovement localPlayer;

    PhotonView pv;

    void Awake()
    {
        Instance=this;
    }
    void Start()
    {
        pv = GetComponent<PhotonView>();
        pausePanel.SetActive(false);
    }

    // void Update()
    // {
    //     if (!pv.IsMine) return;

    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         if (isPaused)
    //             Resume();
    //         else
    //             Pause();
    //     }
    // }

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetLocalPlayerActive(false);
    }

    public void Resume()
    {
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


    public void RegisterLocalPlayer(PlayerMovement player)
    {
        localPlayer = player;
    }


    void SetLocalPlayerActive(bool value)
    {
        //PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (localPlayer != null)
        {
            localPlayer.enabled = value;
            // Debug.Log("player movemnet assighnd");
            
        }
        // else
        // {
        //     Debug.Log("player movement not assighned");
        // }
            

        // //CharacterAiming aiming = FindFirstObjectByType<CharacterAiming>();
        // if (aiming != null)
        //     aiming.enabled = value;
    }
} 