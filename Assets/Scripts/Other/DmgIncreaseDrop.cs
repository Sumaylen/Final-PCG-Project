using UnityEngine;

public class DmgIncreaseDrop : MonoBehaviour
{
    [SerializeField]
    private float damageMultiplier = 1.5f;
    [SerializeField]
    private float buffDuration = 10f;

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player") == true)
        {
            PlayerStats playerStats = player.GetComponentInParent<PlayerStats>();
            playerStats.ApplyDamageBuff(damageMultiplier, buffDuration);
            Destroy(gameObject);
        }
    }

}

