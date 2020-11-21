﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        Vector2 scrollPos;

        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;

        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;

        const float canvasXSize = 4000;
        const float canvasYSize = 500;
        
        //If we want to use a background Image for the Dialogue Editor Window
        //const float backgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //Opens a dockable (un-dockable if utility = true) window
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        //The annotation "OnOpenAsset" gets called whenever we open the asset
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            //The selectionChanged is an event
            Selection.selectionChanged += OnSelectionChanged;

            //Makes the GUI of the node look fancy
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            //Makes the GUI of the player speaking node look fancy
            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);

        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        /// <summary>
        /// Allows us to draw on the Dialogue Editor.
        /// Reference methods in Unity Documentation, under EditorGUI
        /// </summary>
        private void OnGUI()
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();

                //Updates the editor with a scrollview 
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                Rect canvas = GUILayoutUtility.GetRect(canvasXSize, canvasYSize);

                //If we want to have a background for the Dialogue Editor
                //Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                //Rect texCoords = new Rect(0, 0, canvasSize/ backgroundSize, canvasSize/ backgroundSize);
                //GUI.DrawTextureWithTexCoords(canvas, backgroundTex,texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if(creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if(deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPos);
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.GetNodeRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPos;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetRectPosition(Event.current.mousePosition + draggingOffset);

                //Similar to Repaint(), updates the GUI so there isn't lag when dragging a node
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPos = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
                
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }

        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if(node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetNodeRect(), style);

            node.SetNodeText(EditorGUILayout.TextField(node.GetText()));

            //Lays out the GUI elements horizontally
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X"))
            {
                deletingNode = node;
            }
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            DrawLinkButtons(node);
            GUILayout.EndHorizontal();



            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("LINK"))
                {
                    linkingParentNode = node;
                }

            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("CANCEL"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildrenIDs().Contains(node.name))
            {
                if (GUILayout.Button("UNLINK"))
                {
                    linkingParentNode.RemoveChildNode(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("CHILD"))
                {
                    linkingParentNode.AddChildNode(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector3(node.GetNodeRect().xMax, node.GetNodeRect().center.y);

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPos = new Vector3(childNode.GetNodeRect().xMin, childNode.GetNodeRect().center.y);
                Vector3 controlPointOffset = endPos - startPos;
                controlPointOffset.y = 0;
                controlPointOffset.x *= .8f;
                Handles.DrawBezier(
                    startPos, endPos, 
                    startPos + controlPointOffset, 
                    endPos - controlPointOffset, 
                    Color.white, null, 4f);
            }
        }


        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode foundNode = null;
            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetNodeRect().Contains(mousePosition))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}