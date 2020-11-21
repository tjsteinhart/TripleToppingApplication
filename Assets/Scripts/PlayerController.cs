﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int moveSpeed = 5;
    [SerializeField] Collider2D interactableRadius;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform playerTransform;

    private Player_Inputs playerInputControls;
    private Vector2 moveAxis;
    private bool isMoving = false;
    private bool interactWithOthers = false;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInputControls = new Player_Inputs();
    }

    private void OnEnable()
    {
        playerInputControls.Enable();

        playerInputControls.Player.Move.performed += PlayerMove;
    }

    private void OnDisable()
    {
        playerInputControls.Disable();    
    }

    private void Update()
    {
        MovePlayer();
    }


    public void PlayerInteract(InputAction.CallbackContext context)
    {
        if (interactWithOthers)
        {
            //Talks to NPC if applicable
            Debug.Log("Interact");
        }
    }

    /// <summary>
    /// Moves the player and flips the sprite depending on the player's direction
    /// </summary>
    private void MovePlayer()
    {
        if (moveAxis == Vector2.zero)
        {
            playerAnimator.SetBool("IsMoving", false);
        }
        else
        {
            playerAnimator.SetBool("IsMoving", true);
        }

        transform.position += new Vector3(
            moveAxis.x * moveSpeed * Time.deltaTime,
            moveAxis.y * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.identity;
        if (moveAxis.x < 0)
        {
            playerTransform.localScale = new Vector2(-1, 1);
        }
        else if (moveAxis.x > 0)
        {
            playerTransform.localScale = new Vector2(1, 1);
        }
    }

    /// <summary>
    /// Input System Move
    /// </summary>
    /// <param name="context"></param>
    public void PlayerMove(InputAction.CallbackContext context)
    {
        moveAxis = context.ReadValue<Vector2>();
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Interactable>() != null)
        {
            other.gameObject.GetComponent<Interactable>().HighlightInteractable();
            interactWithOthers = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Interactable>() != null)
        {
            collision.gameObject.GetComponent<Interactable>().UnHighlightInteractable();

        }
    }

}