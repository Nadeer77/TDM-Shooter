using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using Photon.Pun;

public class CharacterAiming : MonoBehaviour
{
    public float turnSpeed = 15f;
    public float aimDuration = 0.2f;
    public Rig aimLayer;

    Camera mainCamera;
    RaycastWeapon weapon;

    bool isAiming;
    PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // ALWAYS get weapon reference
        weapon = GetComponentInChildren<RaycastWeapon>();

        if (!photonView.IsMine)
            return;

        mainCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0, yawCamera, 0),
            turnSpeed * Time.fixedDeltaTime
        );
    }

    void Update()
    {
        // INPUT ONLY FOR LOCAL PLAYER
        if (photonView.IsMine)
        {
            if(Input.GetButtonDown("Fire1") && isAiming)
            {
                weapon.StartFiring();
            }

            if(weapon.isFiring)
            {
                weapon.UpdateFiring(Time.deltaTime);
            }

            if(Input.GetButtonUp("Fire1"))
            {
                weapon.StopFiring();
            }

            // ⭐ CAMERA AIM FIX (THIS IS THE IMPORTANT PART)
            Vector3 targetPoint =
                mainCamera.transform.position +
                mainCamera.transform.forward * 100f;

            weapon.raycastDestination.position = targetPoint;
        }

        // AIM LAYER UPDATE FOR EVERYONE
        if (isAiming)
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }

        aimLayer.weight = Mathf.Clamp01(aimLayer.weight);

        // BULLET UPDATE FOR EVERYONE
        weapon.UpdateBullets(Time.deltaTime);
    }

    // ===== NEW INPUT SYSTEM CALLBACK =====
    public void OnAim(InputValue value)
    {
        if (!photonView.IsMine)
            return;

        bool newAimState = value.Get<float>() > 0.5f;

        photonView.RPC("RPC_SetAimState", RpcTarget.All, newAimState);
    }

    [PunRPC]
    void RPC_SetAimState(bool aiming)
    {
        isAiming = aiming;
    }
}