using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerStats stats;
    private float currentHealth;

    private bool isDead = false;

    public static event Action PlayerDied;

    private Text healthText;

    private Image healthFill;

    private Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = stats.GetMaxHealth();
        stats.OnHealthChanged += addHealth;
        UpdateUI();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        healthFill = GameObject.Find("HealthFill").GetComponent<Image>();
    }

    public void addHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, stats.GetMaxHealth());
        UpdateUI();
        // pass this player object to PlayerData so its health can be saved
        PlayerData.Instance.SavePlayer(gameObject);
    }

    void OnDisable()
    {
        stats.OnHealthChanged -= addHealth;
    }


    public void TakeDamage(float amount)
    {
        if (isDead == true)
        {
            return;
        }
        animator.SetTrigger("Damage");
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, stats.GetMaxHealth());
        UpdateUI();
        // pass this player object to PlayerData so its health can be saved
        PlayerData.Instance.SavePlayer(gameObject);
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    void Die()
    {
        animator.SetTrigger("Death");
        isDead = true;
        Debug.Log("Player has died.");
    }

    public void GameOver()
    {
        PlayerDied.Invoke();
    }

    void UpdateUI()
    {
        healthText.text = Mathf.CeilToInt(currentHealth).ToString();
        healthFill.fillAmount = currentHealth / stats.GetMaxHealth();
    }

    // get and set methods
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return stats.GetMaxHealth();
    }

    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, stats.GetMaxHealth());
        UpdateUI();
    }

}
