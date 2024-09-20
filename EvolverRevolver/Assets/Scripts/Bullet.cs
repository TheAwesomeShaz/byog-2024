using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f; // Speed of the bullet

    private void Start()
    {
        Destroy(this.gameObject,3f);
    }

    void Update()
    {
        // Move the bullet in the forward direction every frame
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name);
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground"))
        {

            //TODO: Call take damage function of enemy
            Destroy(gameObject);
        }
    }

}
