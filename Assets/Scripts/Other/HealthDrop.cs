using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    [SerializeField]
    private float healAmount = 20f;

// heal player if they walk in and consume the health drop
    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player") == true)
        {
            PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();

            // return if player is already at max health
            if (playerHealth.GetCurrentHealth() >= playerHealth.GetMaxHealth())
            {
                return;
            }

            playerHealth.addHealth(healAmount);
            Destroy(gameObject);
        }
    }
}
