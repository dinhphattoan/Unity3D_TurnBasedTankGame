using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (rigidbody)
        {
            TankHealth rigidbody1 = rigidbody.GetComponent<TankHealth>();
            if(rigidbody1)
            {
                rigidbody1.earnHealth();
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.

        // Go through all the colliders...
       
    }
}
