using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMapping : MonoBehaviour
{

    //Cam
    public InputActionReference cameraRefrence;
    public string InputAxis0Name;
    public FloatValue[] PropagateInputAxis0;
    public string InputAxis1Name;
    public FloatValue[] PropagateInputAxis1;
    [Space]
    //Movement
    public InputActionReference moveReference;
    public string InputAxis2Name;
    public FloatValue[] PropagateInputAxis2;
    public string InputAxis3Name;
    public FloatValue[] PropagateInputAxis3;
    [Space]
    //Flap
    public InputActionReference flapReference;
    public string InputButton0Name;
    public FloatValue[] PropagateInputButton0;
    [Space]
    //Interact
    public InputActionReference interactReference;
    public string InputButton1Name;
    public FloatValue[] PropagateInputButton1;
    public string InputButton2Name;
    public FloatValue[] PropagateInputButton2;
    public string InputButton3Name;
    public FloatValue[] PropagateInputButton3;


    private void Update()
    {
        if (PropagateInputAxis0 != null)
            foreach (var i in PropagateInputAxis0)
                i.Value = cameraRefrence.action.ReadValue<Vector2>().x;

        if (PropagateInputAxis1 != null)
            foreach (var i in PropagateInputAxis1)
                i.Value = cameraRefrence.action.ReadValue<Vector2>().y;

        if (PropagateInputAxis2 != null)
            foreach (var i in PropagateInputAxis2)
                i.Value = -moveReference.action.ReadValue<Vector2>().x;

        if (PropagateInputAxis3 != null)
            foreach (var i in PropagateInputAxis3)
                i.Value = moveReference.action.ReadValue<Vector2>().y;

        if (PropagateInputButton0 != null)
            foreach (var i in PropagateInputButton0)
                i.Value = flapReference.action.ReadValue<float>();

        if (PropagateInputButton1 != null)
            foreach (var i in PropagateInputButton1)
                i.Value = interactReference.action.ReadValue<bool>() ? 1.0f : 0.0f;

        if (PropagateInputButton2 != null)
            foreach (var i in PropagateInputButton2)
                i.Value = Input.GetButton(InputButton2Name) ? 1.0f : 0.0f;
    }
}
