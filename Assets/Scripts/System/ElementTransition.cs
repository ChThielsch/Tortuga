/*
 
DESCRIPTION
 
Step 1: Click "SET END POSITION". The transition will end here with the given values. 
        By clicking the Button, you also save the "Default position" to reset Gameobject do END POSITION after defining START POSITION

Step 2: Set up position, scale and color for the startpoint. Click "SET START POSITION" to save the values.


Step 3: Click "RESET POSITION" to restore the default position.
*/


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using Cinemachine;

public class ElementTransition : MonoBehaviour
{
    [Header("ANIMATIONS")]
    [HideInInspector]
    public int animationStep;
    [HideInInspector]
    public int maxAnimationSteps;




    [Header("CURVE")]
    public AnimationCurve curve;
    [HideInInspector]
    [Tooltip("Transition duration in Seconds")]
    public float duration = 2f;


    //true if the animation needs to be played while time.deltatime=0
    [Header("PAUSE MENU")]
    public bool useInPauseMode;

    [HideInInspector]
    public bool transformTargetMode;




    [Header("MOVE TO TARGET")]
    [HideInInspector]
    public bool moveToTargt;
    [HideInInspector]
    public Transform target;


    private Coroutine co;
    private Coroutine loopCoroutine;

    [HideInInspector]
    public bool editMode;

    public bool playOnAwake;


    [HideInInspector]
    [Header("LIST POSITION")]
    public List<Vector3> positions;

    [HideInInspector]
    [Header("LIST ROTATION")]
    public List<Quaternion> rotations;

    [HideInInspector]
    [Header("LIST SCALES")]
    public List<Vector3> scales;

    [HideInInspector]
    [Header("LIST COLORS")]
    public List<Color> colors;
    [HideInInspector]
    public Color color01;

    [HideInInspector]
    public Vector3 pos1;

    [HideInInspector]
    public Quaternion rotation01;


    [HideInInspector]
    [Header("LIST DURATIONS")]
    public List<float> durations;

    [HideInInspector]
    [Header("LIST ANIMATION CURVES")]
    public List<AnimationCurve> curves;


    [System.Serializable]
    public class AnimationEvent
    {
        public string animationEventName;
        public bool onAnimationStart;
        public int animationStep;
        public UnityEvent animationEvent;
    }

    public AnimationEvent[] animationevents;

    public UnityEvent call;
    internal Action callback = null;


    [HideInInspector]
    public int callStep;


    public bool excludeColor;
    public bool excludePosition;

    public bool hasExitTime;
    private bool running;
    private bool deactivateWhenFinished = false;

    public bool loop;
    public Vector2 loopPoints;

    public bool cameraMode;

    private CinemachineVirtualCamera cam;

    private void Start()
    {

        if (cameraMode)
        {
            cam = GetComponent<CinemachineVirtualCamera>();
        }

        if (playOnAwake)
        {
            PlayAnimation((int)loopPoints.y);
        }
    }

    private void OnEnable()
    {
        if (playOnAwake)
        {
            PlayAnimation((int)loopPoints.y);
        }
    }

    public void PlayAnimation(int index)
    {
        if (gameObject.activeInHierarchy)
        {
            animationStep = index;

            if (!hasExitTime || !running)
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }

                if (loopCoroutine != null)
                {
                    StopCoroutine(loopCoroutine);
                }

                //this.callback = callback;

                co = StartCoroutine(Transition(index));
            }
        }
    }

    public void PlayAnimationAndDeactivate(int index)
    {
        deactivateWhenFinished = true;
        PlayAnimation(index);
    }

    public void ActivateAndPlayAnimation(int index)
    {
        gameObject.SetActive(true);
        PlayAnimation(index);
    }

    public void JumpToPosition(int index)
    {

        animationStep = index;

        if (cameraMode)
        {
            
        }

        if (!excludePosition)
        {
            transform.localPosition = positions[index];

        }
        transform.localRotation = rotations[index];
        transform.localScale = scales[index];


        if (!excludeColor)
        {
            if (GetComponent<Image>() != null)
            {
                GetComponent<Image>().color = colors[index];
            }

            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color = colors[index];
            }

            if (GetComponent<TextMeshProUGUI>() != null)
            {
                GetComponent<TextMeshProUGUI>().color = colors[index];
            }
        }

    }


    [ContextMenu("Move To Target")]
    public void PlayMoveToTargetAnimation()
    {
        if (gameObject.activeInHierarchy)
        {

            if (co != null)
            {
                StopCoroutine(co);
            }

            if (loopCoroutine != null)
            {
                StopCoroutine(loopCoroutine);
            }

            co = StartCoroutine(TransitionToTarget());
        }
    }

    public void LoopAnimation()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
        }

        loopCoroutine = StartCoroutine(LoopingAnimation((int)loopPoints.x, (int)loopPoints.y));
    }

    IEnumerator LoopingAnimation(int point00, int point01)
    {
        while (true)
        {
            co = StartCoroutine(Transition(point01));
            yield return new WaitForSeconds(durations[point01]);
            co = StartCoroutine(Transition(point00));
            yield return new WaitForSeconds(durations[point00]);
        }
    }




    IEnumerator Transition(int index)
    {
        float timer = 0.0f;

        running = true;

        Vector3 scale01 = transform.localScale;




        //Check and Call AnimationEvents when animation starts
        if (animationevents.Length > 0)
        {
            foreach (AnimationEvent item in animationevents)
            {
                if (item.animationStep == animationStep && item.onAnimationStart)
                {
                    item.animationEvent.Invoke();
                }
            }
        }


        if (!excludePosition)
        {
            pos1 = transform.localPosition;

        }

        Quaternion rotation01 = transform.localRotation;


        if (!excludeColor)
        {
            if (GetComponent<SpriteRenderer>() != null)
            {
                color01 = GetComponent<SpriteRenderer>().color;
            }

            if (GetComponent<Image>() != null)
            {
                color01 = GetComponent<Image>().color;
            }

            if (GetComponent<TextMeshProUGUI>() != null)
            {
                color01 = GetComponent<TextMeshProUGUI>().color;
            }
        }

        while (timer < durations[index])
        {
            if (!excludePosition)
            {
                transform.localPosition = Vector3.Lerp(pos1, positions[index], curves[index].Evaluate(timer / durations[index]));

            }
            transform.localRotation = Quaternion.Lerp(rotation01, rotations[index], curves[index].Evaluate(timer / durations[index]));
            transform.localScale = Vector3.Lerp(scale01, scales[index], curves[index].Evaluate(timer / durations[index]));


            if (!excludeColor)
            {
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().color = Vector4.Lerp(color01, colors[index], curves[index].Evaluate(timer / durations[index]));
                }

                if (GetComponent<SpriteRenderer>() != null)
                {
                    GetComponent<SpriteRenderer>().color = Vector4.Lerp(color01, colors[index], curves[index].Evaluate(timer / durations[index]));
                }

                if (GetComponent<TextMeshProUGUI>() != null)
                {
                    GetComponent<TextMeshProUGUI>().color = Vector4.Lerp(color01, colors[index], curves[index].Evaluate(timer / durations[index]));
                }
            }

            if (!useInPauseMode)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
            }

            yield return null;
        }




        StopCoroutine(co);




        var tmpCallback = callback;
        callback = null;
        tmpCallback?.Invoke();

        JumpToPosition(index);



        running = false;
        if (deactivateWhenFinished)
        {
            gameObject.SetActive(false);
            deactivateWhenFinished = false;
        }

        //Check and Call AnimationEvents when animation ends

        if (animationevents.Length > 0)
        {
            foreach (AnimationEvent item in animationevents)
            {
                if (item.animationStep == animationStep && !item.onAnimationStart)
                {
                    item.animationEvent.Invoke();
                }
            }
        }


        if(index !=loopPoints.x && index != loopPoints.y)
        {
            loop = false;
        }

        if (loop)
        {
            if (animationStep == loopPoints.x)
            {
                PlayAnimation((int)loopPoints.y);
            }
            else
            {
                PlayAnimation((int)loopPoints.x);
            }
        }
    }


    IEnumerator TransitionToTarget()
    {
        float timer = 0.0f;

        Vector3 scale01 = transform.localScale;

        if (!excludePosition)
        {
            pos1 = transform.localPosition;

        }
        rotation01 = transform.localRotation;

        while (timer < duration)
        {
            if (!excludePosition)
            {
                transform.localPosition = Vector3.Lerp(pos1, target.localPosition, curve.Evaluate(timer / duration));

            }
            transform.localRotation = Quaternion.Lerp(rotation01, target.localRotation, curve.Evaluate(timer / duration));
            transform.localScale = Vector3.Lerp(scale01, target.localScale, curve.Evaluate(timer / duration));




            timer += Time.deltaTime;

            yield return null;
        }
        if (animationStep == callStep)


            StopCoroutine(co);
    }




    public void ToggleEditmode(bool active)
    {
        editMode = active;
    }

    public void ToggleTargetMode(bool active)
    {
        transformTargetMode = active;
    }

    public void AddAnimation(int index)
    {

        if (GetComponent<RectTransform>() == null)
        {
            positions.Add(transform.localPosition);
            rotations.Add(transform.localRotation);
            scales.Add(transform.localScale);
        }
        else
        {
            positions.Add(GetComponent<RectTransform>().localPosition);
            rotations.Add(GetComponent<RectTransform>().localRotation);
            scales.Add(GetComponent<RectTransform>().localScale);
        }

        durations.Add(duration);
        curves.Add(new AnimationCurve(curve.keys));

        if (!excludeColor)
        {
            if (GetComponent<Image>() != null)
            {
                colors.Add(GetComponent<Image>().color);
            }
            if (GetComponent<SpriteRenderer>() != null)
            {
                colors.Add(GetComponent<SpriteRenderer>().color);
            }
            if (GetComponent<TextMeshProUGUI>() != null)
            {
                colors.Add(GetComponent<TextMeshProUGUI>().color);
            }
        }

        maxAnimationSteps += index;

    }

    public void OverrideAnimation()
    {

        if (GetComponent<RectTransform>() == null)
        {
            positions[animationStep] = transform.localPosition;
            rotations[animationStep] = transform.localRotation;
            scales[animationStep] = transform.localScale;
        }
        else
        {
            positions[animationStep] = GetComponent<RectTransform>().anchoredPosition;
            rotations[animationStep] = GetComponent<RectTransform>().localRotation;
            scales[animationStep] = GetComponent<RectTransform>().localScale;
        }


        durations[animationStep] = duration;
        curves[animationStep] = new AnimationCurve(curve.keys);

        if (!excludeColor)
        {
            if (GetComponent<Image>() != null)
            {
                colors[animationStep] = GetComponent<Image>().color;
            }
            if (GetComponent<SpriteRenderer>() != null)
            {
                colors[animationStep] = GetComponent<SpriteRenderer>().color;
            }
            if (GetComponent<TextMeshProUGUI>() != null)
            {
                colors[animationStep] = GetComponent<TextMeshProUGUI>().color;
            }
        }

    }

    public void ResetAnimation()
    {
        maxAnimationSteps = 0;
        positions.Clear();
        rotations.Clear();
        scales.Clear();
        colors.Clear();
        durations.Clear();
        curves.Clear();
    }

    public void DeleteAnimation()
    {
        positions.RemoveAt(animationStep);
        rotations.RemoveAt(animationStep);
        scales.RemoveAt(animationStep);
        durations.RemoveAt(animationStep);
        curves.RemoveAt(animationStep);
        colors.RemoveAt(animationStep);
        maxAnimationSteps--;
    }

    public void JumpToLastPosition()
    {
        if (animationStep == 0)
        {
            animationStep = maxAnimationSteps - 1;
        }
        else
        {
            animationStep--;
        }

        if (GetComponent<RectTransform>() == null)
        {
            if (!excludePosition)
            {
                transform.localPosition = positions[animationStep];

            }
            transform.localRotation = rotations[animationStep];
            transform.localScale = scales[animationStep];
        }
        else
        {
            if (!excludePosition)
            {
                GetComponent<RectTransform>().localPosition = positions[animationStep];

            }
            GetComponent<RectTransform>().localRotation = rotations[animationStep];
            GetComponent<RectTransform>().localScale = scales[animationStep];
        }



        duration = durations[animationStep];
        curve.keys = curves[animationStep].keys;

        if (!excludeColor)
        {
            if (GetComponent<Image>() != null)
            {
                GetComponent<Image>().color = colors[animationStep];
            }
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color = colors[animationStep];
            }
            if (GetComponent<TextMeshProUGUI>() != null)
            {
                GetComponent<TextMeshProUGUI>().color = colors[animationStep];
            }
        }


    }

    public void JumpToNextPosition()
    {
        if (animationStep == maxAnimationSteps - 1)
        {
            animationStep = 0;
        }
        else
        {
            animationStep++;
        }


        if (GetComponent<RectTransform>() == null)
        {
            if (!excludePosition)
            {
                transform.localPosition = positions[animationStep];

            }
            transform.localRotation = rotations[animationStep];
            transform.localScale = scales[animationStep];
        }
        else
        {
            if (!excludePosition)
            {
                GetComponent<RectTransform>().localPosition = positions[animationStep];

            }
            GetComponent<RectTransform>().localRotation = rotations[animationStep];
            GetComponent<RectTransform>().localScale = scales[animationStep];
        }


        duration = durations[animationStep];
        curve.keys = curves[animationStep].keys;

        if (!excludeColor)
        {
            if (GetComponent<Image>() != null)
            {
                GetComponent<Image>().color = colors[animationStep];
            }
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color = colors[animationStep];
            }
            if (GetComponent<TextMeshProUGUI>() != null)
            {
                GetComponent<TextMeshProUGUI>().color = colors[animationStep];
            }
        }
    }

    public void SetCallStep()
    {
        callStep = animationStep;
    }

    public void ToggleLoop(bool state)
    {
        loop = state;
    }

    public void Automation()
    {
        if (animationStep == 0)
        {
            PlayAnimation(1);
            animationStep = 1;
        }
        else if (animationStep == 1)
        {
            PlayAnimation(0);
            animationStep = 0;
        }
    }
}
