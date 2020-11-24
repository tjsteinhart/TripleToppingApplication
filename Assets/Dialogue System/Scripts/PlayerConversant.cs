using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue;
        DialogueNode currentNode = null;

        Animator playerAnimator;

        bool isChoosing = false;

        public event Action onConversationUpdated;  

        private void Start()
        {
            playerAnimator = GetComponent<Animator>();
        }

        public void StartDialogue(Dialogue newDialogue)
        {
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            onConversationUpdated();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        /// <summary>
        /// Displays the first line of text, routinely called by NPC interacted on by the player
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if(currentNode == null)
            {
                return "";
            }
            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if(numPlayerResponses > 0)
            {
                playerAnimator.SetBool("IsTalking", true);
                isChoosing = true;
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            if(children.Length > 0)
            {
                int randomNode = UnityEngine.Random.Range(0, children.Count());
                currentNode = children[randomNode];
                playerAnimator.SetBool("IsTalking", false);
                onConversationUpdated();
                return;
            }
            QuitDialogue();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        public void QuitDialogue()
        {
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            GetComponent<PlayerController>().PlayerInputs().Enable();
            playerAnimator.SetBool("IsTalking", false);

            onConversationUpdated();

        }
    }
}
