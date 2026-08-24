using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds ShootButtonState to the existing Shoot button in Scene01, so it greys
/// out while a roll is in progress instead of staying clickable-looking.
/// </summary>
static class ShootButtonWiring
{
    const string TargetSceneName = "Scene01";
    const string ButtonPath = "Canvas/Panel/Button";
    const string RunOnceKey = "ShootButtonWiring.applied.v1";

    [MenuItem("Tools/Bowling/Wire Shoot Button State")]
    public static void Run()
    {
        Apply();
    }

    [InitializeOnLoadMethod]
    static void AutoRunOnce()
    {
        if (EditorPrefs.GetBool(RunOnceKey, false))
            return;

        EditorApplication.delayCall += () =>
        {
            if (EditorPrefs.GetBool(RunOnceKey, false))
                return;

            EditorPrefs.SetBool(RunOnceKey, true);
            Apply();
        };
    }

    static void Apply()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != TargetSceneName)
        {
            Debug.LogWarning($"[ShootButtonWiring] Active scene is '{scene.name}', expected " +
                             $"'{TargetSceneName}'. Open it and run " +
                             "Tools > Bowling > Wire Shoot Button State.");
            return;
        }

        var buttonObject = GameObject.Find(ButtonPath);
        if (buttonObject == null)
        {
            Debug.LogError($"[ShootButtonWiring] '{ButtonPath}' not found in the scene.");
            return;
        }

        var button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"[ShootButtonWiring] '{ButtonPath}' has no Button component.");
            return;
        }

        var state = buttonObject.GetComponent<ShootButtonState>();
        if (state == null)
            state = Undo.AddComponent<ShootButtonState>(buttonObject);

        var so = new SerializedObject(state);
        so.FindProperty("shootButton").objectReferenceValue = button;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[ShootButtonWiring] ShootButtonState wired onto the Shoot button. " +
                  "Scene is dirty - save it if you want to keep the change.");
    }
}
