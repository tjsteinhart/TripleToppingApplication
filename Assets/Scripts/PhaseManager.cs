using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    public enum StatePhase
    {
        First,
        Second
    }

    private StatePhase currentState;
    public StatePhase GetCurrentState() => currentState;

    public delegate void PhaseAction(StatePhase phase);
    public static event PhaseAction onPhaseChanged;

    public void ChangeActionState(StatePhase state)
    {
        currentState = state;
        if (onPhaseChanged != null)
        {
            onPhaseChanged.Invoke(state);
        }
    }
}
