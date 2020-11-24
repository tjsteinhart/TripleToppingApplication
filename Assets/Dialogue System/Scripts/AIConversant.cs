using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] List<Dialogue> aiDialogues;
        [SerializeField] Animator animator;

        PlayerConversant playerConversant;

        private void Awake()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            animator = GetComponent<Animator>();
        }

        public void StartDialogue()
        {
            playerConversant.StartDialogue(ChooseDialogue());
        }

        private void OnEnable()
        {
            playerConversant.onConversationUpdated += TalkAnimation;

        }

        private void OnDisable()
        {
            playerConversant.onConversationUpdated -= TalkAnimation;
        }


        private Dialogue ChooseDialogue()
        {
            Dialogue chosenDialogue = new Dialogue();
            chosenDialogue = aiDialogues[0];

            if (aiDialogues[0].IsOneTimeDialogue())
            {
                aiDialogues.Remove(aiDialogues[0]);
            }
            return chosenDialogue;
        }

        private void TalkAnimation()
        {
            if (playerConversant.HasNext() && !playerConversant.IsChoosing())
            {
                animator.SetBool("NPCTalking", true);
            }
            else
            {
                animator.SetBool("NPCTalking", false);
            }
        }
    }

}
