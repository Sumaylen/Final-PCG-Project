using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;


public class PauseMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel1;

    [SerializeField]
    private LogMetrics logMetrics;
    [SerializeField]
    private TextMeshProUGUI logMetricsText;


    private bool paused = false;

    void Start()
    {
        ResumeGame();
    }

    void Update()
    {
        if (GameManagerUI.gameOver == true)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused == true)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void OnResumeButton()
    {
        ResumeGame();
    }

    // reutrns to main menu
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }


    void PauseGame()
    {
        paused = true;
        panel1.SetActive(true);
         logMetricsText.text = logMetrics.GetMetricsText();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    void ResumeGame()
    {
        paused = false;
        panel1.SetActive(false);

        if (LevelUpUi.LevelUpOpen == true)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
