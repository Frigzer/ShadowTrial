using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ShadowUIStylePreview
{
    [MenuItem("Tools/Shadow Trial/Apply UI Style Preview")]
    public static void ApplyToOpenScenes()
    {
        ShadowUIStyle.StyleSceneCanvases();

        foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            EditorUtility.SetDirty(canvas.gameObject);
        }

        foreach (Graphic graphic in Object.FindObjectsByType<Graphic>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            EditorUtility.SetDirty(graphic.gameObject);
        }

        EditorSceneManager.MarkAllScenesDirty();
    }
}
