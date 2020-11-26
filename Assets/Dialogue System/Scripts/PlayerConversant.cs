using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;
        Dialogue currentDialogue;
        DialogueNode currentNode = null;

        Animator playerAnimator;
        AIConversant currentConversant = null;

        bool isChoosing = false;

        public event Action onConversationUpdated;

        public string GetPlayerName() => playerName;

        private void Start()
        {
            playerAnimator = GetComponent<Animator>();
        }

        public void StartDialogue(AIConversant conversant, Dialogue newDialogue)
        {
            currentConversant = conversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();

            if (currentNode.IsPlayerSpeaking())
            {
                playerAnimator.SetBool("IsTalking", true);
                currentConversant.TalkAnimation(false);
            }
            else
            {
                playerAnimator.SetBool("IsTalking", false);
                currentConversant.TalkAnimation(true);
            }

            TriggerEnterAction();
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

        public string GetAIConversantName()
        {
            return currentConversant.GetSpeakerName();
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
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if(numPlayerResponses > 0)
            {
                playerAnimator.SetBool("IsTalking", true);
                currentConversant.TalkAnimation(false);
                isChoosing = true;
                TriggerExtiAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            if(children.Length > 0)
            {
                int randomNode = UnityEngine.Random.Range(0, children.Count());
                TriggerExtiAction();
                currentNode = children[randomNode];
                TriggerEnterAction();
                playerAnimator.SetBool("IsTalking", false);
                currentConversant.TalkAnimation(true);
                onConversationUpdated();
                return;
            }
            //FinishedCurrentDialogue();
            QuitDialogue();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        private void TriggerEnterAction()
        {
            if(currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExtiAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }

        }

        private void TriggerAction(string action)
        {
            if(action == "") { return; }

            foreach(DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }

        }

        /// <summary>
        /// Used when the player quits out of a dialogue.  Will run in the middle of or at the end of a dialogue.
        /// </summary>
        public void QuitDialogue()
        {
            if (!HasNext())
            {
                currentConversant.SetNextDialogue();
            }

            currentDialogue = null;
            TriggerExtiAction();
            currentNode = null;
            isChoosing = false;
            currentConversant.TalkAnimation(false);
            currentConversant.HighlightInteractable();
            playerAnimator.SetBool("IsTalking", false);
            currentConversant = null;
            onConversationUpdated();
            GetComponent<PlayerController>().PlayerInputs().Enable();
        }

        /// <summary>
        /// Will only be called at the end of a dialogue.  If the AI's dialogue os a one-time dialogue,
        /// the AI will change it's target dialogue.
        /// </summary>
        public void FinishedCurrentDialogue()
        {
            currentConversant.SetNextDialogue();
            QuitDialogue();
        }
    }
}
