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
    public void TakeDamage(int damage, int attackerID)
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
            Die(attackerID);
        }
    }

    void Die(int attackerID)
    {
        Debug.Log("Player Died");

        Debug.Log("Killed by player: " + attackerID);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill(attackerID);
        }

        // Reset health
        currentHealth = maxHealth;

        // Respawn player
        transform.position = spawnPosition;
    }
}