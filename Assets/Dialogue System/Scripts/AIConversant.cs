using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Dialogue
{
    public class AIConversant : MonoBehaviour, Interactable
    {
        [SerializeField] List<AIDialogue> aiDialogues;
        [SerializeField] int aiDialogueIndex = 0;
        [SerializeField] Animator animator;
        [SerializeField] string speakerName;

        AIDialogue currentChoices;
        Dialogue currentDialogue;

        public Dialogue GetCurrentDialogue() => currentDialogue;
        public string GetSpeakerName() => speakerName;

        [SerializeField] Material highlightMaterial;
        [SerializeField] string materialBool;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            currentDialogue = new Dialogue();
        }

        private void OnEnable()
        {
            PhaseManager.onPhaseChanged += SelectNewDialogues;
        }

        private void OnDisable()
        {
            PhaseManager.onPhaseChanged -= SelectNewDialogues;
        }

        private void Start()
        {
            SelectNewDialogues(PhaseManager.Instance.GetCurrentStateInt());
            UnHighlightInteractable();
        }

        private void SelectNewDialogues(int phase)
        {
            aiDialogueIndex = 0;
            foreach (AIDialogue choice in aiDialogues)
            {
                if (choice.time == (PhaseManager.StatePhase)phase)
                {
                    currentChoices = choice;
                    currentDialogue = choice.aiDialogues[aiDialogueIndex];
                }
            }
        }

        public void SetNextDialogue()
        {
            if (currentDialogue.IsOneTimeDialogue())
            {
                aiDialogueIndex += 1;
                currentDialogue = currentChoices.aiDialogues[aiDialogueIndex];
            }
        }

        public void TalkAnimation(bool isTalking)
        {
            animator.SetBool("NPCTalking", isTalking);
        }

        public void HighlightInteractable()
        {
            highlightMaterial.SetInt(materialBool, 1);
        }

        public void UnHighlightInteractable()
        {
            highlightMaterial.SetInt(materialBool, 0);
        }

    }

    /// <summary>
    /// Allows for characters to store multiple dialogues dependent on the PhaseManager state 
    /// </summary>
    [System.Serializable]
    public class AIDialogue
    {
        [SerializeField] public PhaseManager.StatePhase time;
        [SerializeField] public List<Dialogue> aiDialogues;
    }

}
