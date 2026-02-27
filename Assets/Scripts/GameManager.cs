using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform spawnLeft;
    public Transform spawnRight;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos;

        // First player in room
        if (PhotonNetwork.PlayerList.Length == 1)
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
}