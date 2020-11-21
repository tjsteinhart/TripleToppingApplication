using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Dialogue currentDialogue;

        /// <summary>
        /// Displays the first line of text, routinely called by NPC interacted on by the player
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if(currentDialogue == null)
            {
                return "";
            }
            return currentDialogue.GetRootNode().GetText();
        }

        public void Next()
        {

        }

        public bool HasNext()
        {
            return true;
        }

    }
}
