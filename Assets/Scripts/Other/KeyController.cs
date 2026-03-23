using UnityEngine;
using TMPro;

public class KeyController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI keyCounterText;

    private static int keysCollected;
    private static int keysToSpawn;

    private void Awake()
    {
        UpdateUI();
    }

    public void KeysSpawned(int amount)
    {
        keysToSpawn = amount;
        keysCollected = 0;
        UpdateUI();
    }

    public void AddKey()
    {
        keysCollected++;
        UpdateUI();
    }

    public static bool HasEnoughKeys()
    {
        return keysCollected >= keysToSpawn;
    }

    private void UpdateUI()
    {
        keyCounterText.text = $"Keys Required: {keysCollected}/{keysToSpawn}";
    }
}
