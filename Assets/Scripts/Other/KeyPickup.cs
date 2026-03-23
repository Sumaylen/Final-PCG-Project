using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private KeyController keyController;

    public void SetKeyController(KeyController controller)
    {
        keyController = controller;
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player") == false)
        {
            return;
        }

        keyController.AddKey();
        Destroy(gameObject);
    }
}
