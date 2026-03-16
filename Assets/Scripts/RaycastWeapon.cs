using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    public bool isFiring = false;

    [Header("Gun Settings")]
    public int fireRate = 25;
    public float bulletSpeed = 1000f;
    public float bulletDrop = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 25;
    int currentAmmo;
    public float reloadTime = 3f;
    bool isReloading = false;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public GameObject reloadText;

    [Header("Effects")]
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform raycastOrigin;
    public Transform raycastDestination;

    public PhotonView playerView;

    public AudioSource muzzleAudio;
    public AudioClip fireSound;

    Ray ray;
    RaycastHit hitInfo;

    float accumulatedTime;
    float maxLifetime = 3f;

    Queue<Bullet> bullets = new Queue<Bullet>();

    void Start()
    {
        currentAmmo = maxAmmo;

        if (!playerView.IsMine)
            return;

        GameObject canvas = GameObject.FindGameObjectWithTag("parent");

        if (canvas != null)
        {
            Transform hud = canvas.transform.Find("HUD");

            if (hud != null)
            {
                ammoText = hud.Find("AmmoText").GetComponent<TMPro.TextMeshProUGUI>();
                reloadText = hud.Find("ReloadText").gameObject;
            }
            else
            {
                Debug.LogError("HUD not found!");
            }
        }
        else
        {
            Debug.LogError("Canvas with tag 'parent' not found!");
        }

        UpdateAmmoUI();

        if (reloadText != null)
            reloadText.SetActive(false);
    }

    void Update()
    {
        if(GameManager.isGameOver)
        {
            StopFiring();
            return;
        }

        if (!playerView.IsMine)
            return;

        // Manual Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading && currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
            }
        }
    }

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;

        return bullet.initialPosition +
               bullet.initialVelocity * bullet.time +
               0.5f * gravity * bullet.time * bullet.time;
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();

        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0f;

        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);

        return bullet;
    }

    public void StartFiring()
    {
        if(GameManager.isGameOver)
            return;

        if (!playerView.IsMine)
            return;

        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        isFiring = true;
        accumulatedTime = 0f;
        FireBullet();
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    public void UpdateFiring(float deltaTime)
    {
        if(GameManager.isGameOver)
            return;

        if (isReloading)
            return;

        accumulatedTime += deltaTime;

        float fireInterval = 1f / fireRate;

        while (accumulatedTime >= fireInterval)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    void FireBullet()
    {
        if(GameManager.isGameOver)
            return;

        if (currentAmmo <= 0)
            return;

        currentAmmo--;

        UpdateAmmoUI();

        Vector3 direction =
            (raycastDestination.position - raycastOrigin.position).normalized;

        GetComponent<PhotonView>().RPC(
            "RPC_FireBullet",
            RpcTarget.All,
            raycastOrigin.position,
            direction
        );

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    [PunRPC]
    void RPC_FireBullet(Vector3 startPosition, Vector3 direction)
    {
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        if (muzzleAudio != null)
        {
            muzzleAudio.PlayOneShot(fireSound);
        }

        Vector3 velocity = direction * bulletSpeed;

        Bullet bullet = CreateBullet(startPosition, velocity);

        bullets.Enqueue(bullet);
    }

    public void UpdateBullets(float deltaTime)
    {
        int bulletCount = bullets.Count;

        for (int i = 0; i < bulletCount; i++)
        {
            Bullet bullet = bullets.Dequeue();

            Vector3 p0 = GetPosition(bullet);

            bullet.time += deltaTime;

            Vector3 p1 = GetPosition(bullet);

            RaycastSegment(p0, p1, bullet);

            if (bullet.time < maxLifetime)
            {
                bullets.Enqueue(bullet);
            }
            else
            {
                Destroy(bullet.tracer.gameObject);
            }
        }
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            // Ignore self hit
            if (hitInfo.collider.transform.root == playerView.transform)
            {
                bullet.tracer.transform.position = end;
                return;
            }

            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            PlayerHealth targetHealth =
                hitInfo.collider.GetComponentInParent<PlayerHealth>();

            if (targetHealth != null && playerView.IsMine)
            {
                PhotonView targetPV = targetHealth.GetComponent<PhotonView>();

                if (targetPV != null)
                {
                    targetPV.RPC(
                        "TakeDamage",
                        RpcTarget.All,
                        10,
                        playerView.Owner.ActorNumber
                    );
                }
            }

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;

        StopFiring();

        if (reloadText != null)
            reloadText.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;

        UpdateAmmoUI();

        if (reloadText != null)
            reloadText.SetActive(false);

        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + "/" + maxAmmo;
        }
    }
}