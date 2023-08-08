using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class StageProfile : MonoBehaviour
{
    [Range(0,9)]public int index;

    //[Tooltip("All objects this Stage is tracking rn.")]
    //public List<StageObject> list = new List<StageObject>();
    [Tooltip("All things this Stage is tracking rn.")]
    public List<StageObjectProfile> stageObjects= new List<StageObjectProfile>();

    public void Load()
    {
        StageManager.Index = index;

        for (int i = 0; i < stageObjects.Count; i++)
            if (stageObjects[i].stageObj!=null) stageObjects[i].LoadProfile();
            
        Refresh();
    }
    public void Save()
    {
        stageObjects.Clear();

        StageObject[] fetchedObjects = Resources.FindObjectsOfTypeAll<StageObject>();

        for (int i = 0; i < fetchedObjects.Length; i++)
            stageObjects.Add(fetchedObjects[i].Save());
        
        Refresh();
    }
    public void SaveIndividual(StageObject obj)
    {
        for (int i = 0; i < stageObjects.Count; i++)
            if(stageObjects[i].stageObj==obj)
            {
                stageObjects[i]= obj.Save();
                return;
            }

        //if its not yet saved
        stageObjects.Add(obj.Save());
    }
    public void Refresh()
    {
        gameObject.name= index.ToString();
    }

    public void SetDirty()
    {
        EditorUtility.SetDirty(this);
        foreach (StageObjectProfile obj in stageObjects)
        {
            if(obj.stageObj!=null)
                EditorUtility.SetDirty(obj.stageObj);
        }
    }
}



[CustomEditor(typeof(StageProfile))]
public class StageProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StageProfile stageProfile = (StageProfile)target;

        if (GUILayout.Button("Save"))
        {
            stageProfile.Save();
            stageProfile.SetDirty();
        }

        GUILayout.Space(35);

        if (GUILayout.Button("Load"))
        {
            stageProfile.Load();
            stageProfile.SetDirty();
            StageManager b = GameObject.FindObjectOfType<StageManager>();
            EditorUtility.SetDirty(b);
        }

        GUILayout.Space(35);

        if (GUILayout.Button("Purge"))
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "You sure there buddy?",
                "Purging this stge will obliterate all saved settings here.\n" +
                "Think of their families...",
                "Do it.", "Abort.");

            if (confirmed)
            {
                stageProfile.stageObjects.Clear();
                stageProfile.SetDirty();
            }
        }
    }
}