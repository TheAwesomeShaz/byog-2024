using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Animator animator;

    [SerializeField] private LayerMask mouseLayerMask;


    [Header("Animation Related")]
    [SerializeField] private float animationCrossfadeDuration = 0.7f;
    [SerializeField] private String idleAnimString; 
    [SerializeField] private String forwardAnimString;
    [SerializeField] private String shootAnimString;

    [SerializeField] private Rig headRig;
    [SerializeField] private Rig rightHandRig;
    [SerializeField] private Rig rightHandShootRig;


    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Vector3 rightHandRotationOffset;

    private Vector3 inputVector;
    private Vector3 mousePos;

    MousePosition currentMousePosition;
    private bool mousePosChanged = true;
    private bool setToForward;


    private void Awake()
    {
        inputManager.OnShoot += InputManager_OnShoot;
    }

    private void InputManager_OnShoot()
    {

        rightHandShootRig.weight = 1f;
        rightHandRig.weight = 0f;

        DOTween.To(() => rightHandShootRig.weight, x => rightHandShootRig.weight = x, 0f, 0.07f) 
           .OnComplete(() =>
           {
               // Step 2: After the shoot rig reaches 0, smoothly set the right hand rig to 1
               DOTween.To(() => rightHandRig.weight, x => rightHandRig.weight = x, 1f, 0.07f); 
           });
    }

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


        mousePos = inputManager.GetMousePos();
        mousePos.y = 0.5f;
        headTarget.position = mousePos;
        rightHandTarget.position = mousePos;
        rightHandTarget.LookAt(mousePos);
        
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleRotation()
    {
        var targetDirection = Vector3.zero;

        // Calculate target direction based on inputVector
        targetDirection = (Vector3.forward * inputVector.z) + (Vector3.right * inputVector.x);
        targetDirection.y = 0; // Keep the player upright

        // Normalize the target direction and check for significant movement
        if (targetDirection.sqrMagnitude > 0.01f) // Use squared magnitude to avoid small floating-point values
        {
            targetDirection.Normalize();

            // Step 1: Calculate the target rotation towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Step 2: Rotate the player towards the target rotation
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