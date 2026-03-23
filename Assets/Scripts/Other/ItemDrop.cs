using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth enemyHealth;
    [SerializeField]
    private GameObject[] itemDropPrefabs;
    [SerializeField, Range(0f, 1f)]
    private float dropChance = 0.15f;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.OnDeath += TryDropItem;
    }

    void OnDisable()
    {
        enemyHealth.OnDeath -= TryDropItem;
    }

    // when an enemy dies it has a chance to drop a random item
    void TryDropItem()
    {
        if (Random.value < dropChance)
        {
            int randomIndex = Random.Range(0, itemDropPrefabs.Length);
            Instantiate(itemDropPrefabs[randomIndex], transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

    }
}
