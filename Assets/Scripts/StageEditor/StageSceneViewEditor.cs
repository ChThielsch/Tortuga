using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public class StageSceneViewEditor
{
    static StageSceneViewEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10, 10, 120, 60));

        if (GUILayout.Button($"Save Stage {StageManager.Index}"))
        {
            StageManager b = GameObject.FindObjectOfType<StageManager>();

            if (b)
            {
                b.SaveCurrent();
                EditorUtility.SetDirty(b.Get(StageManager.Index));
            }
            else Debug.LogError("No StageManager found in Scene.");
        }

        GUILayout.EndArea();

        Handles.EndGUI();
    }
}
#endif
