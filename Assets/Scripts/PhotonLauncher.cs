using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI connectedText;
    void Start()
    {
        connectedText.gameObject.SetActive(false);
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        connectedText.gameObject.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room Successfully");
        PhotonNetwork.LoadLevel("GameScene");
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