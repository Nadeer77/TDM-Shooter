using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform spawnLeft;
    public Transform spawnRight;

    Dictionary<int, int> playerScores = new Dictionary<int, int>();

    void Awake()
{
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Destroy(gameObject);
    }
}

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos;

        // First player in room
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPos = spawnLeft.position;
        }
        else
        {
            spawnPos = spawnRight.position;
        }

        PhotonNetwork.Instantiate(
            "Player 1",
            spawnPos,
            Quaternion.identity
        );
    }

    public void AddKill(int attackerID)
    {
        if (!playerScores.ContainsKey(attackerID))
        {
            playerScores[attackerID] = 0;
        }

        playerScores[attackerID]++;

        Debug.Log("Player " + attackerID + " Score: " + playerScores[attackerID]);
    }
}