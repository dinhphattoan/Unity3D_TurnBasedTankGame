using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public GameManager gameManager;
    private void Start()
    {
        if(gameManager==null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (rigidbody)
        {
            TankHealth rigidbody1 = rigidbody.GetComponent<TankHealth>();
            if (rigidbody1)
            {
                gameManager.earnShieldPlayer();
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
