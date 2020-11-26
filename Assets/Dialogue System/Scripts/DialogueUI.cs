using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using TMPro;
using UnityEngine.UI;

namespace DialogueUI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI aiText;
        [SerializeField] Button nextButton;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choiceButtonPrefab;
        [SerializeField] Button quitButton;
        [SerializeField] GameObject aiResponse;
        [SerializeField] TextMeshProUGUI aiSpeakerName;

        private void Awake()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        // Start is called before the first frame update
        void Start()
        {
            nextButton.onClick.AddListener(Next);
            quitButton.onClick.AddListener(Quit);
            
        }

        private void OnEnable()
        {
            UpdateUI();
            playerConversant.onConversationUpdated += UpdateUI;
            
        }

        private void OnDisable()
        {
            playerConversant.onConversationUpdated -= UpdateUI;
        }

        private void Next()
        {
            playerConversant.Next();
        }

        private void Quit()
        {
            playerConversant.QuitDialogue();
        }

        private void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }
            aiResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());
            aiSpeakerName.text = playerConversant.GetAIConversantName();
            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = playerConversant.GetText().ToString();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
                Debug.Log("Test");
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform child in choiceRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject newChoice = Instantiate(choiceButtonPrefab, choiceRoot);
                newChoice.GetComponentInChildren<TextMeshProUGUI>().text = choice.GetText();
                Button button = newChoice.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
