using UnityEngine;

public class MoveSpeedIncreaseDrop : MonoBehaviour
{
     [SerializeField]
    private float moveSpeedMultiplier = 1.5f;
    [SerializeField]
    private float buffDuration = 10f;

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player") == true)
        {
            PlayerStats playerStats = player.GetComponentInParent<PlayerStats>();
            playerStats.ApplyMoveSpeedBuff(moveSpeedMultiplier, buffDuration);
            Destroy(gameObject);
        }
    }
}
