using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPun
{
    public int maxHealth = 100;
    float lastDamageTime = 0f;
    float damageCooldown = 0.15f;
    public int currentHealth;

    Vector3 spawnPosition;

    void Start()
    {
        currentHealth = maxHealth;
        spawnPosition = transform.position;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine)
            return;

        // ⭐ Prevent multiple hits in same frame burst
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        currentHealth -= damage;

        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");

        // Reset health
        currentHealth = maxHealth;

        // Respawn player
        transform.position = spawnPosition;
    }
}