using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioSource _audioSource;
    private int _sceneCount;
    [SerializeField] private float thrustForce = 2000f;
    [SerializeField] private float rotationForce = 200f;
    [SerializeField] private float sceneLoadTime = 1f;
    [SerializeField] private AudioClip engineAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip winAudioClip;
    
    private enum State
    {
       Alive,
       Dying,
       Transcending
    }

    private State _state = State.Alive;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        InitScenes();
    }
    
    private void InitScenes()
    {
        _sceneCount = SceneManager.sceneCountInBuildSettings;
    }

    void Update()
    {
        if (_state == State.Alive)
        {
            ProcessRotation();
            ProcessThrust();
        }
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
            ApplyThrust();
        }
        else if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        _rb.AddRelativeForce(Time.deltaTime * thrustForce * Vector3.up);
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(engineAudioClip);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_state != State.Alive){return;}
        
        if (collision.gameObject.GetComponent<Friendly>() == null)
        {
            StartDeathSequence();
        }
        else if (collision.gameObject.GetComponent<Finish>() != null)
        {
            StartSuccessSequence();
        }
        else
        {
            Debug.Log("Friendly :)");
        }
    }

    private void StartDeathSequence()
    {
        // Reload level upon death
        _state = State.Dying;
        _audioSource.Stop(); // Kill the sound if dying
        _audioSource.PlayOneShot(deathAudioClip);
        Invoke(nameof(ReloadScene), sceneLoadTime);
    }

    private void StartSuccessSequence()
    {
        // Reload level upon death
        _state = State.Transcending;
        _audioSource.Stop(); // Stop other sound
        _audioSource.PlayOneShot(winAudioClip);
        Invoke(nameof(LoadNextScene), sceneLoadTime);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % _sceneCount);
    }
}
