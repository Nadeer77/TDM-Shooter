using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance;

    void Awake()
    {
        // Singleton
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
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting to Photon...");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room Successfully");
        PhotonNetwork.LoadLevel("GameScene");
    }

    // ---------- LEAVE ROOM ----------
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        SceneManager.LoadScene("HomeScene");
    }

    public void CreateRoom(string roomId, string password)
    {
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("Room ID is empty");
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        // Store password as custom room property
        Hashtable customProps = new Hashtable();
        customProps.Add("pwd", password);

        roomOptions.CustomRoomProperties = customProps;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "pwd" };

        PhotonNetwork.CreateRoom(roomId, roomOptions);
    }

    public void JoinRoom(string roomId)
    {
        if (string.IsNullOrEmpty(roomId))
        {
            Debug.LogError("Room ID is empty");
            return;
        }

        PhotonNetwork.JoinRoom(roomId);
    }
}