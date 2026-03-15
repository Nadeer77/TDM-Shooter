using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    [Header("Spawn Points")]
    public Transform spawnLeft;
    public Transform spawnRight;

    Dictionary<int, int> playerScores = new Dictionary<int, int>();

    [Header("Score UI")]
    public TextMeshProUGUI player1Score;
    public TextMeshProUGUI player2Score;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultText;

    int winScore = 5;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SpawnPlayer();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos;

        if (PhotonNetwork.IsMasterClient)
            spawnPos = spawnLeft.position;
        else
            spawnPos = spawnRight.position;

        PhotonNetwork.Instantiate(
            "Player 1",
            spawnPos,
            Quaternion.identity
        );
    }

    // Called when a player kills another player
    public void AddKill(int attackerID)
    {
        photonView.RPC("RPC_AddKill", RpcTarget.All, attackerID);
    }

    // Runs on ALL clients
    [PunRPC]
    void RPC_AddKill(int attackerID)
    {
        if (!playerScores.ContainsKey(attackerID))
            playerScores[attackerID] = 0;

        playerScores[attackerID]++;

        UpdateScoreUI();

        CheckWin(attackerID);
    }

    void UpdateScoreUI()
    {
        int p1Score = 0;
        int p2Score = 0;

        if (playerScores.ContainsKey(1))
            p1Score = playerScores[1];

        if (playerScores.ContainsKey(2))
            p2Score = playerScores[2];

        if (player1Score != null)
            player1Score.text = p1Score.ToString("00");

        if (player2Score != null)
            player2Score.text = p2Score.ToString("00");
    }

    void CheckWin(int attackerID)
    {
        if (playerScores[attackerID] >= winScore)
        {
            ShowGameOver(attackerID);
        }
    }

    void ShowGameOver(int winnerID)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        int myID = PhotonNetwork.LocalPlayer.ActorNumber;

        if (myID == winnerID)
        {
            resultText.text = "YOU WIN";
            resultText.color = new Color(0f, 1f, 0.45f); // green
        }
        else
        {
            resultText.text = "YOU LOSE";
            resultText.color = new Color(1f, 0.23f, 0.23f); // red
        }
    }

    // Button function
    public void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("HomeScene");
    }
}