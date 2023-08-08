using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageManager : MonoBehaviour
{
    public static int Index;

    [Tooltip("Save current Stage when loading another from the inspector.")]
    public bool autoSave;

    public StageProfile[] profiles;
    
    public void Load(int index)
    {
        Index = index;

        StageProfile current = Get(Index);

        if (current)
            current.Load();
    }
    public void SaveCurrent()
    {
        StageProfile current = Get(Index);

        if(current)
            current.Save();
    }

    public StageProfile Get(int i)
    {
        if(profiles!=null&&profiles.Length>0)
        foreach (StageProfile b in profiles)
        {
            if (b.index == i)
                return b;
        }
        return null;
    }
}

[CustomEditor(typeof(StageManager))]
public class StageManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StageManager b = (StageManager)target;

        int index= StageManager.Index;
        StageProfile current= b.Get(index);

        EditorGUILayout.LabelField("Current Stage: "+index);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntSlider(index, 0, 9);
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button($"Save current Stage {index}"))
        {
            b.SaveCurrent();
            current.SetDirty();
        }

        GUILayout.Space(20);

        for (int i = 0; i < 10; i++)
        {
            if (GUILayout.Button($"Load Stage {i}"))
            {
                if(b.autoSave)
                {
                    b.SaveCurrent();
                    current.SetDirty();
                }

                b.Load(i);
                current.SetDirty();
                EditorUtility.SetDirty(b);
            }
        }

    }
}