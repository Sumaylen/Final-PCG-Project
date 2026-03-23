using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private PlayerStats stats;
    [SerializeField]
    private LayerMask enemyLayer;

    // Auto-attack radius around the player
    [SerializeField]
    private float meleeRadius = 1.5f;

    [SerializeField]
    private GameObject meleeHitEffect;

    private Animator animator;
    private bool isAttacking = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // trigger attack aniamtion when attack button is pressed
    public void OnAttack(InputAction.CallbackContext context)
    {   
        // only trigegr aniamtion if button is pressed and the attack aniamtion isnt already going out
        if (context.performed == true && isAttacking == false)
        {
            isAttacking = true;
            animator.SetFloat("AttackSpeed", stats.GetAttackSpeedMultiplier());
            animator.SetTrigger("Attack");
        }
        else
        {
            return;
        }
    }

    // apply melee damage to enemies in range
    public void MeleeDamage()
    {   
        // Check for enemies in range
        Vector3 origin = transform.position + Vector3.up;
        Collider[] hits = Physics.OverlapSphere(origin, meleeRadius, enemyLayer, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {   
             // Apply damage
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            enemy.TakeDamage(stats.GetMeleeDamage());
        }
    }

    // called at the end of the animation to indicate that the attack is over
    public void EndAttack()
    {
        isAttacking = false;
    }


    //hit VFX (called by animation event)
    public void SpawnAttackVFX()
    {
        Vector3 spawnPos = transform.position + transform.forward + Vector3.up;
        Instantiate(meleeHitEffect, spawnPos, transform.rotation);
    }


    // Visualize attack radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up;
        Gizmos.DrawWireSphere(origin, meleeRadius);
    }

}
