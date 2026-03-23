using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] 
    private int nextStage = 2;

    // load next stage when player enetrs portal
    private void OnTriggerEnter(Collider entity)
    {
        if (entity.CompareTag("Player"))
        {   
            PlayerData.Instance.SavePlayer(entity.gameObject);
            PlayerData.Instance.SaveStage();
            SceneManager.LoadScene(nextStage);
        }
    }
}
