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

        if (!photonView.IsMine)
            return;

        mainCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weapon = GetComponentInChildren<RaycastWeapon>();
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
        if (!photonView.IsMine)
            return;

        if (isAiming)
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }
        aimLayer.weight = Mathf.Clamp01(aimLayer.weight);

        if(Input.GetButtonDown("Fire1"))
        {
            weapon.StartFiring();
        }

        if(weapon.isFiring)
        {
            weapon.UpdateFiring(Time.deltaTime);
        }

        weapon.UpdateBullets(Time.deltaTime);

        if(Input.GetButtonUp("Fire1"))
        {
            weapon.StopFiring();
        }
    }

    // ===== NEW INPUT SYSTEM CALLBACK =====
    public void OnAim(InputValue value)
    {
        if (!photonView.IsMine)
            return;

        isAiming = value.Get<float>() > 0.5f;
    }
}