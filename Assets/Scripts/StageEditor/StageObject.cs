using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageObject : MonoBehaviour
{
    [Header("Saved Data:\n" +
            "- active\n" +
            "- position\n" +
            "- rotation\n" +
            "- scale\n" +
            "- customFloat\n" +
            "- material\n" +
        "")]
    [SerializeField] string devNote;

    [Tooltip("Ignores being loaded. Is still saving.")]
    [SerializeField] bool isLocked;

    public virtual float CustomFloat
    {
        get => customFloat;
        set => customFloat = value;
    }
    [Space(15)]
    [SerializeField] float customFloat;

    public StageObjectProfile Save()
    {
        StageObjectProfile p = new StageObjectProfile();
        return p.SaveProfile(this);
    }
    public void Load(StageObjectProfile profile)
    {
        if(!isLocked)
        profile.LoadProfile(this);
    }
}

[CustomEditor(typeof(StageObject))]
public class StageObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StageObject o = (StageObject)target;

        GUILayout.Space(35);

        for (int i = 0; i < 10; i++)
        {
            if (GUILayout.Button($"Save on Stage {i}"+(StageManager.Index==i?" (Current)":"")))
            {
                StageManager b = GameObject.FindObjectOfType<StageManager>();

                if (b)
                {
                    StageProfile profile = b.Get(i);
                    if (profile)
                    {
                        profile.SaveIndividual(o);
                        EditorUtility.SetDirty(profile);
                    }
                    else Debug.LogError("No StageProfile with that index assigned to StageManager");
                }
                else Debug.LogError("No StageManager found in Scene.");
            }
        }
    }
}


[System.Serializable]public class StageObjectProfile
{
    public StageObject stageObj;

    public bool active=true;

    public Vector3
        position= Vector3.zero,
        rotation=Vector3.zero,
        scale=Vector3.one;

    public float customFloat = 0;

    public Material
        material=null;

    public StageObjectProfile SaveProfile(StageObject sObj)
    {
        stageObj = sObj;
        GameObject obj = sObj.gameObject;
        active = obj.activeSelf;

        Transform transform = obj.transform;
        position=transform.position;
        rotation=transform.rotation.eulerAngles;
        scale=transform.localScale;

        customFloat = sObj.CustomFloat;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer) material = renderer.sharedMaterial;

        return this;
    }

    public void LoadProfile() => LoadProfile(stageObj);
    public void LoadProfile(StageObject Bobj)
    {
        Debug.Log(position);

        GameObject obj = Bobj.gameObject;
        obj.SetActive(active);

        Transform transform = obj.transform;
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
       transform.localScale = scale;

        Bobj.CustomFloat = customFloat;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer && material) renderer.sharedMaterial = material;
    }
}