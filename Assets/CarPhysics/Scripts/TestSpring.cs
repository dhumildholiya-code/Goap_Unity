using CarPhysics;
using TMPro;
using UnityEngine;

public class TestSpring : MonoBehaviour
{
    [SerializeField] private float strength;
    [Header("Spring")]
    [SerializeField] private float stiffness;
    [SerializeField] private float damping;

    private CustomSpring _spring;
    private Vector3 _startPosition;
    private Vector3 _startScale;

    private void Start()
    {
        _startPosition = transform.position;
        _startScale = transform.localScale;
        _spring = new CustomSpring(stiffness, damping);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _spring.SetGoal(-1f);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _spring.SetGoal(0f);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _spring.SetGoal(1f);
        }

        float value = _spring.Evaluate(Time.deltaTime);

        transform.position = _startPosition + Vector3.right * value * strength;
        transform.localScale = _startScale + Vector3.one * Mathf.Abs(value) * strength;
    }
}
