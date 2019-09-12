using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float thrustForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        
        // Thrust control
        if (Input.GetButton("Jump"))
        {
            rb.AddRelativeForce(Vector3.up * thrustForce);
        }
        
        // Rotation control
        if (horizontal > 0)
        {
        }
        else if (horizontal < 0)
        {
        }
    }
}
