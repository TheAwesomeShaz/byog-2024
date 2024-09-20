using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Animator animator;
    [SerializeField] private Gun gun;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldownTime;


    [SerializeField] private LayerMask mouseLayerMask;
    [SerializeField] private CharacterController characterController;

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

    private bool mousePosChanged = true;
    private bool setToForward;
    private float currentSpeed;
    private bool canDash = true;

    private void Awake()
    {
        gun.OnBulletShot += Gun_OnBulletShot;
        currentSpeed = moveSpeed;
    }

    private void Gun_OnBulletShot()
    {
        HandleGunRecoilAnim();
    }


    private void HandleGunRecoilAnim()
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
        HandleMovementUpdate();
        HandleRotation();
        HandleAnimations();
        HandleDash();
    }

    private void HandleDash()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (canDash)
            {
                canDash = false;
                StartCoroutine(Dash());
                IEnumerator Dash()
                {
                    float startTime = Time.time;
                    while(Time.time < startTime + dashTime)
                    {
                        currentSpeed = dashSpeed;

                        yield return null;
                    }
                    currentSpeed = moveSpeed;
                    yield return new WaitForSeconds(dashCooldownTime);
                    canDash = true;

                }
            }
        }
    }

    private void HandleMovementUpdate()
    {
        if (inputVector != Vector3.zero)
        {
           characterController.Move(transform.forward * currentSpeed * Time.deltaTime);
        }
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



    public void SetDashSpeed(float dashSpeed)
    {
        this.dashSpeed = dashSpeed;
    }

    /// <summary>
    /// Affects the Length of the Dash
    /// </summary>
    public void SetDashTime(float dashTime)
    {
        this.dashTime = dashTime;
    }

    public void SetDashCooldown(float dashCooldownTime)
    {
        this.dashCooldownTime = dashCooldownTime;
    }


}