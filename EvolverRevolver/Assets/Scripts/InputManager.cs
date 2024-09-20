using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] LayerMask mouseMask;
    public event Action OnShoot;

    public Vector2 GetRawMoveVector()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnShoot?.Invoke();
        }
    }

    public Vector3 GetMousePos()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, mouseMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
