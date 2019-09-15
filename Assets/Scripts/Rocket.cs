using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioSource _audioSource;
    [SerializeField] private float thrustForce;
    [SerializeField] private float rotationForce;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessRotation();
        ProcessThrust();
    }

    private void ProcessRotation()
    {
        var horizontal = Input.GetAxis("Horizontal");

        // Take manual control of rotation
        _rb.freezeRotation = true;
        
        // Rotation control
        if (horizontal > 0)
        {
            transform.Rotate(Time.deltaTime * rotationForce * -Vector3.forward);
        }
        else if (horizontal < 0)
        {
            transform.Rotate(Time.deltaTime * rotationForce * Vector3.forward);
        }
        
        // Resume physics control of rotation
        _rb.freezeRotation = false;
    }

    private void ProcessThrust()
    {
// Thrust control
        if (Input.GetButton("Jump"))
        {
            _rb.AddRelativeForce(Time.deltaTime * thrustForce * Vector3.up);
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
