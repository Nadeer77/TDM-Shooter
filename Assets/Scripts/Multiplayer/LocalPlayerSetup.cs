using Photon.Pun;
using UnityEngine;

public class LocalPlayerSetup : MonoBehaviour
{
    void Start()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
    }
}