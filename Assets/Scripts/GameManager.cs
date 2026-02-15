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
        Vector3 spawnPosition;

        // First player joins
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            spawnPosition = spawnLeft.position;
        }
        else
        {
            spawnPosition = spawnRight.position;
        }

        PhotonNetwork.Instantiate(
            "Player",
            spawnPosition,
            Quaternion.identity
        );
    }
}