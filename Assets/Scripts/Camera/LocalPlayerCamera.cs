using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class LocalPlayerCamera : MonoBehaviour
{
    void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();

        if (!pv.IsMine)
            return;

        CinemachineCamera cam =
            FindAnyObjectByType<CinemachineCamera>();

        if (cam == null)
        {
            Debug.LogError("No CinemachineCamera found in scene!");
            return;
        }

        Transform target = transform.Find("CameraTarget");

        if (target == null)
        {
            Debug.LogError("CameraTarget not found on Player!");
            return;
        }

        cam.Follow = target;
        cam.LookAt = target;
    }
}