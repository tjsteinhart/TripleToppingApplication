using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using Dialogue;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int moveSpeed = 5;
    [SerializeField] Collider2D interactableRadius;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform playerTransform;

    private Player_Inputs playerInputControls;
    private Vector2 moveAxis;
    private bool interactWithOthers = false;
    GameObject interactable = null;
    PlayerConversant playerConversant = null;

    [SerializeField] Light2D fieldOfView;
    [SerializeField] float dayTime = 20f;
    [SerializeField] float nightTime = 7f;
    [SerializeField] float lightChangeDuration = 2f;


    public Player_Inputs PlayerInputs() => playerInputControls;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInputControls = new Player_Inputs();
        playerConversant = GetComponent<PlayerConversant>();
    }

    private void OnEnable()
    {
        PhaseManager.onPhaseChanged += LightChanger;
        playerInputControls.Enable();

        playerInputControls.Player.Move.performed += PlayerMove;
        playerInputControls.Player.Interact.started += PlayerInteract;
        playerInputControls.Player.Quit.started += QuitGame;
    }

    private void QuitGame(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        PhaseManager.onPhaseChanged -= LightChanger;
        playerInputControls.Disable();    
    }

    private void Update()
    {
        MovePlayer();
    }

    public void PlayerInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerInputControls.Disable();
            if (interactWithOthers)
            {
                if (interactable.GetComponent<AIConversant>())
                {
                    var aiConversant = interactable.GetComponent<AIConversant>();
                    playerConversant.StartDialogue(aiConversant, aiConversant.GetCurrentDialogue());
                    aiConversant.UnHighlightInteractable();
                    return;
                }
            }
            
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
            interactable = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Interactable>() != null)
        {
            collision.gameObject.GetComponent<Interactable>().UnHighlightInteractable();
            interactWithOthers = false;
            interactable = null;
        }
    }

    /// <summary>
    /// Used to adjust the 2D light focused on the player
    /// </summary>
    public void LightChanger(int phase)
    {
        if(phase % 2 == 0)
        {
            StartCoroutine(ChangeOuterRadius(nightTime, dayTime, lightChangeDuration));
        }
        else
        {
            StartCoroutine(ChangeOuterRadius(dayTime, nightTime, lightChangeDuration));
        }
    }

    public IEnumerator ChangeOuterRadius(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            fieldOfView.pointLightOuterRadius = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fieldOfView.pointLightOuterRadius = v_end;
    }
}