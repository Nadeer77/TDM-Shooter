using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PhotonLauncher Instance;

    [Header("UI")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI texteey;
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
        // parent=GameObject.FindWithTag("parent").GetComponent<Transform>();
        // texteey=Instantiate(statusText,parent);
        // texteey.gameObject.SetActive(false);

        ShowMessage("Connecting to Photon");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded+=LoadedScene;
    }
    public override void OnDisable()
    {
        SceneManager.sceneLoaded-=LoadedScene;
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

    // YOU joined
    public override void OnJoinedRoom()
    {
        if(texteey==null) Debug.Log("test is null when enteringn room");
        //ShowMessage(PhotonNetwork.NickName + " joined the room");
        Invoke("ShowMsgWhenJoinedRoom",2);
        

        PhotonNetwork.LoadLevel("GameScene");
    }

    void ShowMsgWhenJoinedRoom()
    {
        Debug.Log(texteey==null);
        ShowMessage(PhotonNetwork.NickName + " joined the room");
    }

    // OTHER player joined
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("called joined room");
        
        ShowMessage(newPlayer.NickName + " joined the room");
    }

    // OTHER player left / disconnected
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ShowMessage(otherPlayer.NickName + " left the room");
    }

    // YOU left
    public override void OnLeftRoom()
    {
        ShowMessage(PhotonNetwork.NickName + " left the room");
        SceneManager.LoadScene("HomeScene");
    }

    // YOU disconnected
    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowMessage(PhotonNetwork.NickName + " disconnected");
        SceneManager.LoadScene("HomeScene");
    }

    // ---------------- ROOM ACTIONS ----------------

    public void CreateRoom(string roomId)
    {
        if (string.IsNullOrEmpty(roomId)) return;

        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomId, options);

        ShowMessage(PhotonNetwork.NickName + " created the room");
    }

    public void JoinRoom(string roomId)
    {
        if (string.IsNullOrEmpty(roomId)) return;
        PhotonNetwork.JoinRoom(roomId);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // ---------------- UI ----------------
    
    void ShowMessage(string msg)
    {
        if (texteey == null)
        {
            Debug.Log("text is null");
            return;
        }
        Debug.Log("txt is not nnull");

        texteey.text = msg;
        texteey.gameObject.SetActive(true);

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 3f);
    }

    void HideMessage()
    {
        if (parent!=null&& statusText != null)
            texteey.gameObject.SetActive(false);
    }

    // handling secons scene
    public void LoadedScene(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 2|| scene.buildIndex==0)
        {
            Debug.Log(scene.buildIndex+" scene number");
          parent=GameObject.FindWithTag("parent").GetComponent<Transform>();
          
          texteey=Instantiate(statusText,parent);
          texteey.gameObject.SetActive(false);
        }
    }
}