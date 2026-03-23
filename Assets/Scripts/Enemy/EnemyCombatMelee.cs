using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatMelee : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float baseMeleeDamage = 10f;
    private float meleeDamage;

    // Attack radius around the enemy (melee range)
    [SerializeField]
    private float meleeRadius = 1.5f;

    // Time between attacks (seconds)
    [SerializeField]
    private float meleeCooldown = 1.0f;
    private float currentMeleeCooldown = 0f;

    // Delay before the hit lands
    [SerializeField]
    private float windupTime = 0.5f;

    [SerializeField]
    private GameObject meleeHitEffect;

    private bool windingUp = false;
    private float windupEndTime = 0f;

    private Animator animator;
    

    void Start()
    {   
        player = GameObject.FindGameObjectWithTag("Player"). transform;
        animator = GetComponent<Animator>();
        meleeDamage = baseMeleeDamage;
    }

    void Update()
    {
      
        // If enemy winding up for an attack, stop them
        if (windingUp == true)
        {
            agent.isStopped = true;

            // Cancel if player left range
            if (IsPlayerInRange() == false)
            {
                windingUp = false;
                agent.isStopped = false;
                return;
            }

            // after windup, do damage 
            if (Time.time >= windupEndTime)
            {
                DoDamage();
                windingUp = false;
                currentMeleeCooldown = Time.time + meleeCooldown;
                agent.isStopped = false;
            }

            return;
        }

        //  if attack is on cooldown, do nothing
        if (Time.time < currentMeleeCooldown)
        {
            return;
        }

        // Start attack if player is in range
        if (IsPlayerInRange() == true)
        {
            // Stop chasing while trying to do cattack
            agent.isStopped = true;
            windingUp = true;
            animator.SetTrigger("Attack");
            windupEndTime = Time.time + windupTime;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    // CHheck if player is within melee attack range
    bool IsPlayerInRange()
    {
        //UnityEngine.Debug.Log("IsPlayerInRange called");
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.position;
        enemyPos.y = 0f;
        playerPos.y = 0f;

        float dist = Vector3.Distance(enemyPos, playerPos);
        return dist <= meleeRadius;
    }

    void DoDamage()
    {
        PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();
        playerHealth.TakeDamage(meleeDamage);
        Vector3 spawnPos = player.position + Vector3.up * 0.5f;
        Instantiate(meleeHitEffect, spawnPos, Quaternion.identity);

    }

    public void IncreaseMeleeDamage(float percent)
    {
        meleeDamage += meleeDamage * (percent / 100f);
    }



    // Visualize attack radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);
    }
}
