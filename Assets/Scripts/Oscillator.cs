using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] private Vector3 movementVector;
    [SerializeField] private float period = 2f;

    private Vector3 _startingPos;
    private float _oscillatePercentage = 0.5f;

    void Start()
    {
        _startingPos = transform.position;
    }

    void Update()
    {
        if (period <= Mathf.Epsilon) {return;}
        
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        _oscillatePercentage = (Mathf.Sin(cycles * tau) / 2f) + 0.5f;

        Vector3 offset = movementVector * _oscillatePercentage;
        transform.position = _startingPos + offset;
    }
}
