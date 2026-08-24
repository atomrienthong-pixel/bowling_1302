using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Buttons on the main menu. Start loads the game scene, Exit quits.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Scene the Start button loads. It has to be listed in Build Settings.")]
    private string gameSceneName = "Scene01";

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("[MainMenu] No game scene name set.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError($"[MainMenu] Scene \"{gameSceneName}\" is not in Build Settings, " +
                           "so it cannot be loaded.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        // Application.Quit does nothing while running inside the editor, so stop
        // play mode instead and the button can still be tested.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
