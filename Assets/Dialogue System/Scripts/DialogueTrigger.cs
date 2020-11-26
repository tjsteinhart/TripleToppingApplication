using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{   
    //Triggers during dialogues, can be called from within a specific node in the dialogue
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] string action;
        [SerializeField] UnityEvent onTrigger;

        public void Trigger(string actionToTrigger)
        {
            if(actionToTrigger == action)
            {
                onTrigger.Invoke();
            }
        }
    }
}
