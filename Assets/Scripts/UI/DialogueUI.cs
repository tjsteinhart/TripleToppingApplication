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
        [SerializeField] TextMeshProUGUI AIText;
        

        private void Awake()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        // Start is called before the first frame update
        void Start()
        {
            AIText.text = playerConversant.GetText().ToString();
        }

        
    }
}
