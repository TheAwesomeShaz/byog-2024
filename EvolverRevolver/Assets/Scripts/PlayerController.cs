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
    private bool setToForward;

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
            setToForward = false;
        }
        else if(!setToForward)
        {
            setToForward = true;
            animator.CrossFade(forwardAnimString, animationCrossfadeDuration);
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
        if (inputVector.magnitude > 0.1f) // Make sure there is meaningful input
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 200f * Time.deltaTime);
        }
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