using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{

    public static UI current;


    public ElementTransition[] transitions;

    void Awake()
    {
        UI.current = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        TransitionIn(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionIn(int index)
    {
        transitions[index].JumpToPosition(1);
        transitions[index].PlayAnimation(0);
    }

    public void TransitionOut(int index)
    {
        transitions[index].PlayAnimation(1);
    }
}
