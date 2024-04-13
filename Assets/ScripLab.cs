using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScripLab : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector3(0, 0, 10));

    }
}
