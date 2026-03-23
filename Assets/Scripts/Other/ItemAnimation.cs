using UnityEngine;

public class ItemAnimation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 45f;
    [SerializeField]
    private float floatHeight = 0.25f;
    [SerializeField]
    private float floatSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        // sine wave for smooth float effect
        float up = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPosition + new Vector3(0f, up, 0f);
    }
}
