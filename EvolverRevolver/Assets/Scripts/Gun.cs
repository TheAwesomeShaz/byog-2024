using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform shootImpactUpPoint;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    private bool isShooting;

    private void Awake()
    {
        inputManager.OnShoot += InputManager_OnShoot;
    }

    private void InputManager_OnShoot()
    {
        isShooting = true;
        Instantiate(bulletPrefab, shootPoint.position,shootPoint.rotation);

        // Get the current local rotation in Euler angles
        Vector3 currentRotation = transform.localEulerAngles;

        // Subtract 45 degrees from the X axis rotation
        currentRotation.x -= 90f;

        // Apply the new rotation back to the transform
        transform.localEulerAngles = currentRotation;

        DOVirtual.DelayedCall(0.07f, () =>
        {
            isShooting = false;
        });
    }

    private void Update()
    {
        if (!isShooting)
        {
            transform.LookAt(inputManager.GetMousePos());
        }
    }

}
