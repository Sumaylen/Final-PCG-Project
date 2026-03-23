using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel1;
    [SerializeField]
    private GameObject panel2;

    void Start()
    {
        ShowMainPanel();
    }

    public void OnStagesButton()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    public void OnBackButton()
    {
        ShowMainPanel();
    }

    void ShowMainPanel()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void OnPlayButton()
    {
        // reset player stats
        PlayerData.Instance.ResetRunData();
        SceneManager.LoadScene(1); // BSP Stage 1
    }

    public void OnStage1Button()
    {   
        // reset player stats
        PlayerData.Instance.ResetRunData();
        SceneManager.LoadScene(1); // BSP Stage 1
    }

    public void OnStage2Button()
    {
        // strts stage 2 directly but with reset stats
        PlayerData.Instance.ResetRunData();
        SceneManager.LoadScene(2); // BSP Stage 2
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
