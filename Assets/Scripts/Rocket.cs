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

    private void OnCollisionEnter(Collision collision)
    {
        if(_state != State.Alive){return;}
        
        if (collision.gameObject.GetComponent<Friendly>() == null)
        {
            // Reload level upon death
            _state = State.Dying;
            _audioSource.Stop();  // Kill the sound if dying
            Invoke(nameof(ReloadScene), sceneLoadTime);
        }
        else if (collision.gameObject.GetComponent<Finish>() != null)
        {
            // Reload level upon death
            _state = State.Transcending;
            Invoke(nameof(LoadNextScene), sceneLoadTime);
        }
        else
        {
            Debug.Log("Friendly :)");
        }
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
