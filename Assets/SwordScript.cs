using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager gameManager;
    private void Start()
    {
        if (gameManager == null)
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
                gameManager.earnATKPlayer();
                Destroy(transform.parent.gameObject);
            }
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
