using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text frameText;

    [SerializeField]
    private TMP_Text messageText;

    [SerializeField]
    private TMP_Text aimText;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private TMP_Text gameOverText;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private float messageShowDuration = 2f;

    private Coroutine messageRoutine;

    private void Awake()
    {
        instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
    }

    public void ShowScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void ShowFrame(int frame, int totalFrame, int roll)
    {
        if (frameText != null)
            frameText.text = "Frame " + frame + "/" + totalFrame + "   Roll " + roll;
    }

    public void ShowAim(float angle)
    {
        if (aimText != null)
            aimText.text = "Aim: " + Mathf.RoundToInt(angle) + "°";
    }

    public void ShowMessage(string message)
    {
        if (messageText == null)
            return;

        messageText.text = message;
        messageText.gameObject.SetActive(true);

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(HideMessage());
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(messageShowDuration);
        messageText.gameObject.SetActive(false);
    }

    public void ShowGameOver(string message)
    {
        if (gameOverText != null)
            gameOverText.text = message;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
