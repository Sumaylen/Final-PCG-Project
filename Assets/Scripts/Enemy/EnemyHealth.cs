using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 50f;
    [SerializeField] 
    private int xpReward = 50;
    private float currentHealth;
    private bool isDead = false;

    public event Action OnDeath;

    private Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead == true)
        {
            return;
        }

        currentHealth -= amount;
        animator.SetTrigger("Damage");
        Debug.Log("Enemy was hit");
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {   
        isDead = true;
        animator.SetTrigger("Death");
        OnDeath();
        GiveXPToPlayer();
        KillCounter.AddKill(1);  
    }

    // gets called as an aniamtion event so isnt immidieately destroyed
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void GiveXPToPlayer()
    {
        GameObject player= GameObject.FindGameObjectWithTag("Player");
        PlayerProgression prog = player.GetComponent<PlayerProgression>();
        prog.AddXP(xpReward);

        //Debug.Log("Player gained xp: " + xpReward);
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    public void IncreaseMaxHealth(float percent)
    {
        float healthPercent = GetHealthPercent();
        maxHealth += maxHealth * (percent / 100f);
        currentHealth = maxHealth * healthPercent;
    }
}
