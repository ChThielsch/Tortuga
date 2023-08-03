using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class EventTrigger : MonoBehaviour, IInteractable
{
    [TextArea] [SerializeField] string devNote;

    public bool isBlocked()
        => done || !ConditionMet();

    [SerializeField] string tooltip;
    public string Tooltip => tooltip;


    #region Trigger

    [Tooltip("Sets itself to done when exceeding this Number.\n" +
        "<=0 is infinite."
        )]
    public float maxTimesTriggered;
    [ShowOnly] public int timesTriggered;
    [Tooltip("Can't be triggered if blocked.")]
    [ShowOnly] public bool done;

    public enum TriggerType
    {
        None, Start, Interact, Enter
    }
    [Tooltip("The type of Trigger:\n" +
        "None - Can only be triggerd manually.\n" +
        "Start - Automatically triggered onStart.\n" +
        "Interact - Triggerd on Interaction (for FP-Controller).\n" +
        "Enter - Triggerd when player collider enters its TriggerCollider."
        )]
    public TriggerType type;

    private void Start()
    {
        if (type != TriggerType.Start) return;
        TriggerWithCondition();
    }
    public void OnInteract()
    {
        if (type != TriggerType.Interact) return;
        TriggerWithCondition();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (type != TriggerType.Enter) return;

        if (other.tag == "Player")
            TriggerWithCondition();
    }

    #endregion

    #region Condition

    public enum TriggerCondition
    {
        None, OtherTrigger, Item, LogIndex, Custom
    }
    [Space]
    public TriggerCondition condition;

    public EventTrigger otherConditionTrigger;

    public bool ConditionMet()
    {
        switch (condition)
        {
            case TriggerCondition.None:
                return true;
            case TriggerCondition.Custom:
                return CustomCondition();

            case TriggerCondition.OtherTrigger:
                if (otherConditionTrigger.done)
                    return true;
                break;
            case TriggerCondition.Item:
                Debug.LogWarning("Item Condition unimplemented");
                return false;
                break;
            case TriggerCondition.LogIndex:
                Debug.LogWarning("Log Index Condition unimplemented");
                return false;
                break;
        }
        return false;
    }
    public virtual bool CustomCondition()
    {
        return true;
    }

    #endregion

    #region Events

    [Space]
    public EventBatch[] batches;

    public enum VarianceMode
    {
        None, Toggle, Cointoss
    }
    [Tooltip("Plays VarianceBatch instead under certain conditions:\n" +
        "None - Never.\n" +
        "Toggle - If timesTriggerd is even.\n" +
        "Cointoss - Random, equal odds."
        )]
    public VarianceMode varianceMode;
    [Tooltip("Plays VarianceBatch instead under certain conditions. See VarianceMode.")]
    public EventBatch[] varianceBatches;

    [Tooltip("Can't be triggered again while already executing events")]
    [ShowOnly][SerializeField] protected bool isExecutingEvents;

    public EventBatch[] GetBatches()
    {
        switch (varianceMode)
        {
            case VarianceMode.Toggle:
                return timesTriggered % 2 == 0 ? varianceBatches : batches;
            case VarianceMode.Cointoss:
                return UnityEngine.Random.value<0.5f ? varianceBatches : batches;
        }
        return batches;
    }
    public void TriggerWithCondition()
    {
        if (!ConditionMet()) return;
        Trigger();
    }
    public void Trigger()
    {
        if (done || isExecutingEvents) return;

        timesTriggered++;
        if (0 < maxTimesTriggered && timesTriggered >= maxTimesTriggered)
            done = true;

        EventBatch[] theseBatches = GetBatches();

        StartCoroutine(TriggerExecute(theseBatches));
    }
    IEnumerator TriggerExecute(EventBatch[] theseBatches, bool replay=false)
    {

        foreach (EventBatch batch in theseBatches)
        {
            if (replay && batch.replayMode == EventBatch.ReplayMode.Ignore)
                continue;

            if(!replay||batch.replayMode==EventBatch.ReplayMode.Kickoff)
                yield return new WaitForSeconds(batch.delay);

            batch.events?.Invoke();
        }
    }

    #endregion
}


[System.Serializable]
public class EventBatch
{
    [SerializeField] string devNote;
    public float delay;

    public enum ReplayMode
    {
       Ignore, Status, Kickoff
    }
    [Tooltip("Behaviour if done==true on Start (usually by means of loading a Save):\n" +
        "Ignore - Don't trigger this batch.\n" +
        "Status - Trigger this batch, ignoring it's delay.\n" +
        "Kickoff - Trigger this batch with delay. Used to reintroduce current state of affairs to player. " +
        "Subsequent batches are delayed regardless of their own replayMode."
        )]
    public ReplayMode replayMode;

    [SerializeField]
    public UnityEvent events;
}