using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


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
    public int fireRate = 25;
    public float bulletSpeed = 1000f;
    public float bulletDrop = 0f;

    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform raycastOrigin;
    public Transform raycastDestination;

    Ray ray;
    RaycastHit hitInfo;

    float accumulatedTime;
    float maxLifetime = 3f;
    PhotonView photonView;


    // ✅ QUEUE INSTEAD OF LIST
    Queue<Bullet> bullets = new Queue<Bullet>();

    // ----------------------------------------------------

    void Start()
    {
        photonView = GetComponentInParent<PhotonView>();

        // Only local player needs aiming
        if (!photonView.IsMine)
            return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found");
            return;
        }

        CrossHairTarget target =
            cam.GetComponentInChildren<CrossHairTarget>();

        if (target == null)
        {
            Debug.LogError("CrossHairTarget not found under Main Camera");
            return;
        }

        raycastDestination = target.transform;
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

    // ----------------------------------------------------

    public void StartFiring()
    {
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
        accumulatedTime += deltaTime;
        float fireInterval = 1f / fireRate;

        while (accumulatedTime >= fireInterval)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    // ----------------------------------------------------

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

            // keep bullet alive
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

    // ----------------------------------------------------

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    // ----------------------------------------------------

    void FireBullet()
    {
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        Vector3 velocity =
            (raycastDestination.position - raycastOrigin.position).normalized
            * bulletSpeed;

        Bullet bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Enqueue(bullet);
    }
}