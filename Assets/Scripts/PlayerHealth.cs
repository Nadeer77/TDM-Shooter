using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviourPun
{
    public int maxHealth = 100;
    public int currentHealth;

    float lastDamageTime = 0f;
    float damageCooldown = 0.15f;

    Image healthBar;
    TextMeshProUGUI respawnText;

    Vector3 spawnPosition;

    bool isDead = false;

    PlayerMovement movement;
    CharacterAiming aiming;
    RaycastWeapon weapon;

    CharacterController controller;
    Renderer[] renderers;

    void Start()
    {
        spawnPosition = transform.position;

        movement = GetComponent<PlayerMovement>();
        aiming = GetComponent<CharacterAiming>();
        weapon = GetComponentInChildren<RaycastWeapon>();

        controller = GetComponent<CharacterController>();
        renderers = GetComponentsInChildren<Renderer>();

        if (!photonView.IsMine)
            return;

        GameObject canvas = GameObject.FindGameObjectWithTag("parent");

        if (canvas != null)
        {
            Transform hud = canvas.transform.Find("HUD");

            if (hud == null)
            {
                Debug.LogError("HUD not found under Canvas!");
                return;
            }

            healthBar = hud.Find("HealthBar/Green").GetComponent<Image>();
            respawnText = hud.Find("RespawnText").GetComponent<TextMeshProUGUI>();
        }

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    [PunRPC]
    public void TakeDamage(int damage, int attackerID)
    {
        if (!photonView.IsMine) return;
        if (isDead) return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die(attackerID);
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar == null) return;

        float healthPercent = (float)currentHealth / maxHealth;

        healthBar.fillAmount = healthPercent;

        if (healthPercent > 0.75f)
            healthBar.color = Color.green; //green
        else if (healthPercent > 0.55f)
            healthBar.color = Color.yellow; // yellow
        else if (healthPercent > 0.35f)
            healthBar.color = new Color(1f, 0.5f, 0f); // orange
        else
            healthBar.color = new Color(0.8f, 0f, 0f); // dark red
    }

    void Die(int attackerID)
    {
        if (isDead) return;

        isDead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill(attackerID);
        }

        photonView.RPC("RPC_HandleDeath", RpcTarget.All);
    }

    [PunRPC]
    void RPC_HandleDeath()
    {
        foreach (Renderer r in renderers)
            r.enabled = false;

        if (controller != null)
            controller.enabled = false;

        if (photonView.IsMine)
        {
            if (movement != null)
                movement.enabled = false;

            if (aiming != null)
                aiming.enabled = false;

            if (weapon != null)
            {
                weapon.StopFiring();
                weapon.enabled = false;
            }

            StartCoroutine(RespawnCoroutine());
        }
    }

    IEnumerator RespawnCoroutine()
    {
        if (respawnText != null)
            respawnText.gameObject.SetActive(true);

        int countdown = 3;

        while (countdown > 0)
        {
            if (respawnText != null)
                respawnText.text = "Respawning in " + countdown;

            yield return new WaitForSeconds(1f);

            countdown--;
        }

        if (respawnText != null)
            respawnText.gameObject.SetActive(false);

        photonView.RPC("RPC_RespawnPlayer", RpcTarget.All, spawnPosition);
    }

    [PunRPC]
    void RPC_RespawnPlayer(Vector3 pos)
    {
        transform.position = pos;

        foreach (Renderer r in renderers)
            r.enabled = true;

        if (controller != null)
            controller.enabled = true;

        if (photonView.IsMine)
        {
            currentHealth = maxHealth;
            isDead = false;

            UpdateHealthUI();

            if (movement != null)
                movement.enabled = true;

            if (aiming != null)
                aiming.enabled = true;

            if (weapon != null)
                weapon.enabled = true;
        }
    }
}