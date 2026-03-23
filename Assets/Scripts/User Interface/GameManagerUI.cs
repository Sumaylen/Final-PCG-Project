using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerUI : MonoBehaviour
{
    public static bool gameOver = false;

    [SerializeField]
    private GameObject gameManagerPanel;
    [SerializeField]
    private TextMeshProUGUI retryButtonText;
    [SerializeField]
    private GameObject gameOverTitle;
    [SerializeField]
    private GameObject youWinTitle;

    private bool playerWon = false;

    void Start()
    {
        HidePanel();
    }

    // event subscribers
    void OnEnable()
    {
        PlayerHealth.PlayerDied += HandlePlayerDied;
        CATimer.PlayerWon += HandlePlayerWon;
    }

    void OnDisable()
    {
        PlayerHealth.PlayerDied -= HandlePlayerDied;
        CATimer.PlayerWon -= HandlePlayerWon;
    }

    public void HandlePlayerDied()
    {
        if (gameOver == true)
        {
            return;
        }

        retryButtonText.text = "RETRY";
        gameOverTitle.SetActive(true);

        // win title only exists on stage 2
        if (youWinTitle != null)
        {
            youWinTitle.SetActive(false);
        }

        PauseGame();
    }

    public void OnRetryButton()
    {
        // resume game after button is pressed
        Time.timeScale = 1f;

        // if player won restart game oltherwise retry current stage
        if (playerWon == true)
        {
            // reset stats
            PlayerData.Instance.ResetRunData();
            SceneManager.LoadScene(1);
        }
        else
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            // if stage 2 load the saved stats otherwise reset stats
            if (currentScene == 2)
            {
                PlayerData.Instance.LoadStage();
            }
            else
            {
                PlayerData.Instance.ResetRunData();
            }

            SceneManager.LoadScene(currentScene);
        }
    }

    public void HandlePlayerWon()
    {
        if (gameOver == true)
        {
            return;
        }

        playerWon = true;
        retryButtonText.text = "RESTART";
        gameOverTitle.SetActive(false);
        youWinTitle.SetActive(true);

        PauseGame();
    }

    // load main menu scene 0
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }

    // enable panel ,cusor  and pause game
    void PauseGame()
    {
        gameOver = true;
        gameManagerPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // panel and title is hidden at the start
    void HidePanel()
    {
        gameOver = false;
        playerWon = false;
        gameManagerPanel.SetActive(false);
        gameOverTitle.SetActive(false);

        if (youWinTitle != null)
        {
            youWinTitle.SetActive(false);
        }

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
