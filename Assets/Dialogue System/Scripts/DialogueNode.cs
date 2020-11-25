using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    
    public class DialogueNode : ScriptableObject
    {
        [TextArea] [SerializeField] string text;
        [SerializeField] List<string> childrenIDs = new List<string>();
        [SerializeField] Rect nodeRect = new Rect(0,0,200,100);

        //If you want to have more than 2 speakers, try using an enum
        [SerializeField] bool isPlayerSpeaking = false;

        //Fields for triggering actions based on nodes
        [SerializeField] string onEnterAction;
        [SerializeField] string onExitAction;

        public string GetText() => text;
        public List<string> GetChildrenIDs() => childrenIDs;
        public Rect GetNodeRect() => nodeRect;

        public bool IsPlayerSpeaking() => isPlayerSpeaking;

        public string GetOnEnterAction() => onEnterAction;
        public string GetOnExitAction() => onExitAction;

//Pre-processor directive, will only include this in the editor, not when the game is being built or running
#if UNITY_EDITOR
        public void SetRectPosition(Vector2 newPos)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            nodeRect.position = newPos;
            EditorUtility.SetDirty(this);
        }

        public void SetNodeText(string updatedText)
        {
            if(updatedText != text)
            {
                Undo.RecordObject(this, "Update Node Text");
                text = updatedText;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetIsPlayerSpeaking(bool playerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = playerSpeaking;
            EditorUtility.SetDirty(this);
        }

        public void AddChildNode(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            childrenIDs.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChildNode(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            childrenIDs.Remove(childID);
            EditorUtility.SetDirty(this);
        }

#endif

    }


}
