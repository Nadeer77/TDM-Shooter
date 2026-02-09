using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class CharacterAiming : MonoBehaviour
{
    public float turnSpeed = 15f;
    public float aimDuration = 0.2f;
    public Rig aimLayer;

    Camera mainCamera;
    RaycastWeapon weapon;

    bool isAiming;

    void Start()
    {
        mainCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weapon = GetComponentInChildren<RaycastWeapon>();
    }

    void FixedUpdate()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0, yawCamera, 0),
            turnSpeed * Time.fixedDeltaTime
        );
    }

    void Update()
    {
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
        isAiming = value.Get<float>() > 0.5f;
    }
}