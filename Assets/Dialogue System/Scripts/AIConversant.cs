using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] List<AIDIalogueSelect> aiDialogues;
    }

    [System.Serializable]
    public class AIDIalogueSelect
    {
        [SerializeField] PhaseManager.StatePhase state;
        [SerializeField] Dialogue dialogue;
    }
}
