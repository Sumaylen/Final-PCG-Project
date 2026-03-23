using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerXPUI : MonoBehaviour
{
    private PlayerProgression progression;

    [SerializeField]
    private Image xpFill;
    [SerializeField]
    private TextMeshProUGUI levelValue;
    [SerializeField]
    private TextMeshProUGUI expAmount;
    // controls how fast the xp bar fills up
    [SerializeField]
    private float fillLerpSpeed = 1f;

    private float currentFill = 0f;
    private float targetFill = 0f;

    void Start()
    {
        progression = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProgression>();
        progression.OnXPChanged += HandleXPChanged;
        int currentXP = progression.GetCurrentXP();
        int xpToNext = progression.GetXPToNext();
        HandleXPChanged(currentXP, xpToNext);
        UpdateLevelText();
        currentFill = targetFill;
        xpFill.fillAmount = currentFill;
    }

    void Update()
    {   
        // xp bar fill smoothing
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * fillLerpSpeed);
        xpFill.fillAmount = currentFill;
    }

    void OnDisable()
    {
        progression.OnXPChanged -= HandleXPChanged;
    }

    void HandleXPChanged(int currentXP, int xpToNext)
    {
        float xpFillAmount = (float)currentXP / xpToNext;
        targetFill = Mathf.Clamp01(xpFillAmount);
        UpdateLevelText();
        UpdateExpText(currentXP, xpToNext);
    }

    void UpdateLevelText()
    {
        levelValue.text = progression.GetLevel().ToString();
    }

    void UpdateExpText(int currentXP, int xpToNext)
    {
        expAmount.text = $"{currentXP}/{xpToNext}";
    }
}
