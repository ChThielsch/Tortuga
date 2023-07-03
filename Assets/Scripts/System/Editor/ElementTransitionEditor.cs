using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ElementTransition))]
public class ElementTransitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ElementTransition transition = (ElementTransition)target;
        var style = new GUIStyle(GUI.skin.button);
        style.hover.textColor = Color.red;
        GUILayout.Label("BUNTSPECHT ELEMENT TRANSITION");
        GUILayout.Space(20);
        GUILayout.Label("CONTROLS");

        if (!Application.isPlaying)
        {
            if (transition.maxAnimationSteps > 1)
            {
                transition.animationStep = EditorGUILayout.IntSlider("STEP" + "  " + transition.animationStep + "/" + (transition.maxAnimationSteps - 1), transition.animationStep, 0, transition.maxAnimationSteps - 1);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<-"))
                {
                    transition.JumpToLastPosition();
                }

                if (GUILayout.Button("->"))

                {
                    transition.JumpToNextPosition();
                }

                GUILayout.EndHorizontal();
            }


            transition.duration = EditorGUILayout.Slider("DURATION", transition.duration, 0, 30);

            if (GUILayout.Button("ADD NEW ANIMATION"))
            {
                transition.AddAnimation(1);
            }

            if (GUILayout.Button("OVERRIDE ANIMATION"))
            {
                transition.OverrideAnimation();
                PrefabUtility.RecordPrefabInstancePropertyModifications(transition);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(transition.gameObject.scene);
            }


            if (GUILayout.Button("SET POSITION AS CALLER"))
            {
                transition.SetCallStep();
            }

            if (GUILayout.Button("DELETE ANIMATION"))
            {
                transition.DeleteAnimation();
            }

            GUILayout.Space(20);

            if (!transition.transformTargetMode)
            {
                GUILayout.Label("MODES");
                if (GUILayout.Button("MOVE TO TARGET"))
                {
                    transition.ToggleTargetMode(true);
                }
            }
            else
            {
                if (GUILayout.Button("INDIVIDUAL MOVEMENT"))
                {
                    transition.ToggleTargetMode(false);
                }
            }

            if (transition.transformTargetMode)
            {
                transition.target = (Transform)EditorGUILayout.ObjectField("TARGET", transition.target, typeof(Transform), true);
            }

            if (GUILayout.Button("CLEAR ANIMATIONS", style))
            {
                transition.ResetAnimation();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (transition.maxAnimationSteps > 1)
            {
                if (GUILayout.Button("PLAY LAST ANIMATION"))
                {


                    if (transition.animationStep == 0)
                    {
                        transition.animationStep = transition.maxAnimationSteps - 1;
                        transition.PlayAnimation(transition.animationStep);
                    }
                    else
                    {
                        transition.animationStep--;
                        transition.PlayAnimation(transition.animationStep);
                    }
                }

                if (GUILayout.Button("PLAY NEXT ANIMATION"))
                {


                    if (transition.animationStep == transition.maxAnimationSteps - 1)
                    {
                        transition.animationStep = 0;
                        transition.PlayAnimation(transition.animationStep);
                    }
                    else
                    {
                        transition.animationStep++;
                        transition.PlayAnimation(transition.animationStep);
                    }
                }
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("PLAY MOVE TO POSITION"))
            {
                transition.PlayMoveToTargetAnimation();
            }
        }
        DrawDefaultInspector();
    }
}