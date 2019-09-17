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
    [SerializeField] private ParticleSystem engineParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private ParticleSystem winParticles;
    
    private enum State
    {
       Alive,
       Dying,
       Transcending
    }

    private State _state = State.Alive;
    private bool _collisionsDisabled = false;

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

        if (Debug.isDebugBuild)
        {
            ProcessDebugKeys();
        }
    }

    private void ProcessDebugKeys()
    {
        if(Input.GetButtonDown("Debug1")){
            LoadNextScene();
        }
        if(Input.GetButtonDown("Debug2")){
            _collisionsDisabled = !_collisionsDisabled;
            Debug.Log("collisions " + (_collisionsDisabled ? "off" : "on"));
        }
    }

    private void ProcessRotation()
    {
        var horizontal = Input.GetAxis("Horizontal");

        // Skip if there's no rotation to process
        if (!(Mathf.Abs(horizontal) > Mathf.Epsilon)) {return;}
        
        // Take manual control of rotation
        _rb.freezeRotation = true;
            
        // Rotation control
        transform.Rotate(
            Time.deltaTime *  // deltaTime for framerate variability correction
            rotationForce *  // the actual rotation multiplier
            (Mathf.Abs(horizontal)/horizontal) *  // determine positive or negative rotation
            -1f *  // invert the rotation direction for more natural controls
            Vector3.forward  // rotate around the z-axis
            );
            
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
            engineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        _rb.AddRelativeForce(Time.deltaTime * thrustForce * Vector3.up);
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(engineAudioClip);
            engineParticles.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_state != State.Alive || _collisionsDisabled){return;}
        
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
        engineParticles.Stop();
        _audioSource.PlayOneShot(deathAudioClip);
        deathParticles.Play();
        Invoke(nameof(ReloadScene), sceneLoadTime);
    }

    private void StartSuccessSequence()
    {
        // Reload level upon death
        _state = State.Transcending;
        _audioSource.Stop(); // Stop other sound
        engineParticles.Stop();
        _audioSource.PlayOneShot(winAudioClip);
        winParticles.Play();
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
