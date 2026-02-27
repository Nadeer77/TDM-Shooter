using UnityEngine;
using Photon.Pun;

public class CrossHairTarget : MonoBehaviour
{
    PhotonView photonView;
    Camera mainCamera;
    Ray ray;
    RaycastHit hitInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        photonView = GetComponentInParent<PhotonView>();

        if (!photonView.IsMine)
            return;

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;
            
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;
        Physics.Raycast(ray, out hitInfo);
        transform.position = hitInfo.point;
    }
}
