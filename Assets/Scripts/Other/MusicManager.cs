using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] 
    private AudioSource explorationAudio;
    [SerializeField] 
    private AudioSource combatAudio;
    [SerializeField] 
    private float fadeSpeed = 0.3f; // higher it is the quicker it fades
    [SerializeField]
    private float maxExplorationVolume = 0.7f;
    [SerializeField] 
    private float maxCombatVolume = 0.7f;

    private bool inCombat = false;
    
    void Start()
    {
        explorationAudio.Play();
        combatAudio.Play();

        explorationAudio.volume = 1f;
        combatAudio.volume = 0f;
    }

    void Update()
    {
        float explorationVolume;
        float combatVolume;

        SetCombatState(EnemyAI.IsAnyEnemyAggro());

        if (inCombat == true)
        {
            explorationVolume = 0f;
            combatVolume = maxCombatVolume;
        }
        else
        {
            explorationVolume = maxExplorationVolume;
            combatVolume = 0f;
        }

        explorationAudio.volume = Mathf.MoveTowards(
            explorationAudio.volume,
            explorationVolume,
            fadeSpeed * Time.deltaTime
        );

        combatAudio.volume = Mathf.MoveTowards(
            combatAudio.volume,
            combatVolume,
            fadeSpeed * Time.deltaTime
        );
    }


    public void SetCombatState(bool combat)
    {
        inCombat = combat;
    }
}
