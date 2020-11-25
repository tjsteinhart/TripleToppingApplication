using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] List<Dialogue> aiDialogues;
        [SerializeField] int aiDialogueIndex = 0;
        [SerializeField] Animator animator;

        PlayerConversant playerConversant;
        Dialogue currentDialogue;

        public Dialogue GetCurrentDialogue() => currentDialogue;

        private void Awake()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            animator = GetComponent<Animator>();
            currentDialogue = new Dialogue();
        }

        private void Start()
        {
            currentDialogue = aiDialogues[aiDialogueIndex];
        }

        public void SetNextDialogue()
        {
            if (currentDialogue.IsOneTimeDialogue())
            {
                aiDialogueIndex += 1;
                currentDialogue = aiDialogues[aiDialogueIndex];
            }
        }

        public void TalkAnimation(bool isTalking)
        {
            animator.SetBool("NPCTalking", isTalking);
        }
    }

}
