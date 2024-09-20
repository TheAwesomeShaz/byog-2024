using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Animator animator;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float animationCrossfadeDuration = 0.7f;
    [SerializeField] private String idleAnimString; 
    [SerializeField] private String forwardAnimString; 
    [SerializeField] private String backAnimString; 
    [SerializeField] private String leftAnimString; 
    [SerializeField] private String rightAnimString; 


    private Vector3 inputVector;
    private Vector3 mousePos;

    MousePosition currentMousePosition;
    private bool mousePosChanged = true;

    private void Update()
    {
        GetInput();
        HandleRotation();
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        if(inputVector == Vector3.zero)
        {
            animator.CrossFade(idleAnimString, animationCrossfadeDuration);
        }

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.D))
        {
            // Aiming below the player
            if (mousePos.z < transform.position.z)
            {
                mousePosChanged = true;
                SetAimingBelowPlayerAnims();
            }

            // Aiming above the player
            if (mousePos.z > transform.position.z)
            {
                mousePosChanged = true;
                SetAimingAbovePlayerAnims();
            }

            // Aiming at Right of the player
            if (mousePos.x > transform.position.x)
            {
                mousePosChanged = true;
                SetAimingRightOfPlayerAnims();
            }

            // Aiming at Left of the Player
            if (mousePos.x < transform.position.x)
            {
                mousePosChanged = true;
                SetAimingLeftOfPlayerAnims();
            }
        }

        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            mousePosChanged = true;
        }


    }

    private void SetAimingBelowPlayerAnims()
    {
        if (inputVector.z > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(backAnimString, animationCrossfadeDuration);
        }
        if (inputVector.z < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(forwardAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(leftAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(rightAnimString, animationCrossfadeDuration);
        }
    }

    private void SetAimingLeftOfPlayerAnims()
    {
        if (inputVector.z > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(rightAnimString, animationCrossfadeDuration);
        }
        if (inputVector.z < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(leftAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(backAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(forwardAnimString, animationCrossfadeDuration);
        }
    }

    private void SetAimingRightOfPlayerAnims()
    {
        if (inputVector.z > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(leftAnimString, animationCrossfadeDuration);
        }
        if (inputVector.z < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(rightAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(forwardAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(backAnimString, animationCrossfadeDuration);
        }
    }

    private void SetAimingAbovePlayerAnims()
    {
        if (inputVector.z > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(forwardAnimString, animationCrossfadeDuration);
        }
        if (inputVector.z < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(backAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x > 0)
        {
            mousePosChanged = false;
            animator.CrossFade(rightAnimString, animationCrossfadeDuration);
        }
        if (inputVector.x < 0)
        {
            mousePosChanged = false;
            animator.CrossFade(leftAnimString, animationCrossfadeDuration);
        }
    }

    private void GetInput()
    {
        inputVector = inputManager.GetRawMoveVector();
        inputVector = new Vector3(inputVector.x, 0, inputVector.y).normalized;


        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit hit, 500f, groundLayer))
        {
            mousePos = hit.point;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleRotation()
    {
        transform.LookAt(mousePos);
    }

    private void HandleMovement()
    {
        rb.MovePosition(transform.position + (inputVector) * moveSpeed * Time.fixedDeltaTime);
    }



}
public enum MousePosition
{
    AbovePlayer,
    BelowPlayer,
    LeftOfPlayer,
    RightOfPlayer,
}