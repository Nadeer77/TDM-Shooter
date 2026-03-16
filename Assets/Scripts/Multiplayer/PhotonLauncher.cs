using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance;

    [Header("UI")]
    public TextMeshProUGUI statusText;          // Prefab
    public TextMeshProUGUI statusTextInstance;  // Runtime instance
    public Transform parent;

    // ---------------- LIFECYCLE ----------------

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ShowMessage("Connecting to Photon");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += LoadedScene;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadedScene;
    }

    // ---------------- CONNECTION ----------------

    public override void OnConnectedToMaster()
    {
        ShowMessage("Connected to Photon");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        ShowMessage("Joined Lobby");
    }

    // ---------------- ROOM ----------------

    public override void OnJoinedRoom()
    {
        Invoke(nameof(ShowMsgWhenJoinedRoom), 1.5f);

        PhotonNetwork.LoadLevel("GameScene");
    }

    void ShowMsgWhenJoinedRoom()
    {
        ShowMessage(PhotonNetwork.NickName + " joined the room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ShowMessage(newPlayer.NickName + " joined the room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ShowMessage(otherPlayer.NickName + " left the room");
    }

    public override void OnLeftRoom()
    {
        ShowMessage(PhotonNetwork.NickName + " left the room");
        SceneManager.LoadScene("HomeScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowMessage("Disconnected from server");
        SceneManager.LoadScene("HomeScene");
    }

    // ---------------- ROOM ACTIONS ----------------

    public void CreateRoom(string roomId)
    {
        if (string.IsNullOrEmpty(roomId)) return;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2
        };

        PhotonNetwork.CreateRoom(roomId, options);

        ShowMessage("Creating room...");
    }

    public void JoinRoom(string roomId)
    {
        if (string.IsNullOrEmpty(roomId)) return;

        PhotonNetwork.JoinRoom(roomId);

        ShowMessage("Joining room...");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // ---------------- ROOM FAILURES ----------------

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowMessage("Room is full, Try to create one");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowMessage("Room already exists. Try another ID.");
    }

    // ---------------- UI ----------------

    void ShowMessage(string msg)
    {
        if (statusTextInstance == null || statusTextInstance.gameObject == null)
        {
            Debug.Log("Status text not ready yet");
            return;
        }

        statusTextInstance.text = msg;
        statusTextInstance.gameObject.SetActive(true);

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 3f);
    }

    void HideMessage()
    {
        if (statusTextInstance != null)
        {
            statusTextInstance.gameObject.SetActive(false);
        }
    }

    // ---------------- SCENE LOADING ----------------

    public void LoadedScene(Scene scene, LoadSceneMode mode)
    {
        GameObject parentObj = GameObject.FindWithTag("parent");

        if (parentObj != null)
        {
            parent = parentObj.transform;

            // Prevent multiple status texts
            if (statusTextInstance == null)
            {
                statusTextInstance = Instantiate(statusText, parent);
                statusTextInstance.gameObject.SetActive(false);
            }
            else
            {
                statusTextInstance.transform.SetParent(parent);
            }
        }
    }
}