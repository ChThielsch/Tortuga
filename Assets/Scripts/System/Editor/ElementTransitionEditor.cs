using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ElementTransition))]
public class ElementTransitionEditor : Editor
{
    private ElementTransition transition;
    private GUIStyle style;

    public override void OnInspectorGUI()
    {
        transition = (ElementTransition)target;
        style = new GUIStyle(GUI.skin.button);
        style.hover.textColor = Color.red;

        GUILayout.Label("CRITICAL RABBIT TRANSITION");
        GUILayout.Space(20);
        GUILayout.Label("CONTROLS");

        if (!Application.isPlaying)
        {
            DrawAnimationControls();
            DrawDurationSlider();
            DrawAnimationButtons();
            DrawPositionButtons();
            DrawTargetModeControls();
            DrawClearAnimationsButton();
        }
        else
        {
            DrawPlayAnimationControls();
            DrawPlayMoveToPositionButton();
        }

        DrawDefaultInspector();
    }

    /// <summary>
    /// Draws the animation step controls.
    /// </summary>
    private void DrawAnimationControls()
    {
        if (transition.maxAnimationSteps > 1)
        {
            transition.animationStep = EditorGUILayout.IntSlider("STEP " + transition.animationStep + "/" + (transition.maxAnimationSteps - 1), transition.animationStep, 0, transition.maxAnimationSteps - 1);

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
    }

    /// <summary>
    /// Draws the duration slider for controlling animation duration.
    /// </summary>
    private void DrawDurationSlider()
    {
        transition.duration = EditorGUILayout.Slider("DURATION", transition.duration, 0, 30);
    }

    /// <summary>
    /// Draws the buttons for adding and overriding animations.
    /// </summary>
    private void DrawAnimationButtons()
    {
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
    }

    /// <summary>
    /// Draws the buttons for setting position as caller and deleting animation.
    /// </summary>
    private void DrawPositionButtons()
    {
        if (GUILayout.Button("SET POSITION AS CALLER"))
        {
            transition.SetCallStep();
        }

        if (GUILayout.Button("DELETE ANIMATION"))
        {
            transition.DeleteAnimation();
        }
    }

    /// <summary>
    /// Draws the controls for toggling target mode (move to target or individual movement).
    /// </summary>
    private void DrawTargetModeControls()
    {
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
    }

    /// <summary>
    /// Draws the button for clearing animations.
    /// </summary>
    private void DrawClearAnimationsButton()
    {
        if (GUILayout.Button("CLEAR ANIMATIONS", style))
        {
            transition.ResetAnimation();
        }
    }

    /// <summary>
    /// Draws the play animation controls.
    /// </summary>
    private void DrawPlayAnimationControls()
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
    }

    /// <summary>
    /// Draws the button for playing the move to position animation.
    /// </summary>
    private void DrawPlayMoveToPositionButton()
    {
        if (GUILayout.Button("PLAY MOVE TO POSITION"))
        {
            transition.PlayMoveToTargetAnimation();
        }
    }
}