using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public event Action OnBulletShot;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform shootImpactUpPoint;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    private bool isShooting;
    [SerializeField] private float fireRate = 0.5f;
    private float timeBtwnShots;
    private bool canShoot;

    private void Awake()
    {
        inputManager.OnShoot += InputManager_OnShoot;
    }

    private void InputManager_OnShoot()
    {
        if (canShoot)
        {
            ShootBullet();
            OnBulletShot?.Invoke();
            HandleGunRotation();
        }
    }

    private void ShootBullet()
    {
        isShooting = true;
        canShoot = false;
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        timeBtwnShots = fireRate;
    }

    private void HandleGunRotation()
    {
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
        
        if(timeBtwnShots <= 0)
        {
            canShoot = true;
        }
        else
        {
            timeBtwnShots -= Time.deltaTime;
        }

      
    }

    public void SetFireRate(float newFireRate)
    {
        fireRate = newFireRate;
    }


}
