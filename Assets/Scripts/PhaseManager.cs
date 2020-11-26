using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : Singleton<PhaseManager>
{
    public enum StatePhase
    {
        Day1 = 0,
        Night1 = 1,
        Day2 = 2
    }

    private StatePhase currentState;
    public StatePhase GetCurrentState() => currentState;
    public int GetCurrentStateInt() => (int)currentState;

    public delegate void PhaseAction(int phase);
    public static event PhaseAction onPhaseChanged;

    private void Start()
    {
        currentState = StatePhase.Day1;
    }

    public void ChangeActionState(int state)
    {
        currentState = (StatePhase)state;
        if (onPhaseChanged != null)
        {
            onPhaseChanged.Invoke(state);
        }
    }

}
