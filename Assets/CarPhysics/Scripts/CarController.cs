using TMPro;
using UnityEngine;

namespace CarPhysics
{
    [System.Serializable]
    public struct Tier
    {
        public Transform transform;
        [Range(0f, 1f)]
        public float gripFactor;
    }
    public class CarController : MonoBehaviour
    {
        [SerializeField] private Transform centerOfMass;
        [Header("Movement")]
        [SerializeField] private float turnSpeed;
        [SerializeField] private float forwardForce;
        [SerializeField] private float breakForce;
        [SerializeField] private float regularFrictionForce;
        [SerializeField] private float maxSteer;
        [SerializeField] private float maxSpeed;
        [SerializeField] private AnimationCurve powerCurve;
        [Header("Tires")]
        [SerializeField] private Tier[] tiers;
        public float tierMass;
        [Header("Suspension")]
        public float restLength = 1f;
        public float springStrength = 30000f;
        public float springDamper = 4000f;

        private Rigidbody _rb;

        private bool _isGrounded;
        private float _forwardMove;
        private float _turnValue;
        private float _angle;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            _angle = 0f;
            _rb.centerOfMass = centerOfMass.localPosition;
        }

        private void Update()
        {
            _forwardMove = Input.GetAxis("Vertical");
            _turnValue = Input.GetAxis("Horizontal");
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < tiers.Length; i++)
            {
                Transform tierTransform = tiers[i].transform;


                Debug.DrawRay(tierTransform.position, -tierTransform.up * restLength, Color.red);
                if (Physics.Raycast(tierTransform.position, -tierTransform.up, out RaycastHit hit, restLength))
                {
                    _isGrounded = true;
                    Debug.DrawLine(tierTransform.position, hit.point, Color.green);

                    // Suspension
                    Vector3 tireWorldVel = _rb.GetPointVelocity(tierTransform.position);
                    float offset = restLength - hit.distance;
                    float vel = Vector3.Dot(tierTransform.up, tireWorldVel);
                    float force = (offset * springStrength) - (vel * springDamper);
                    _rb.AddForceAtPosition(tierTransform.up * force, hit.point);

                    //Steering
                    // apply rotation on forward tiers.
                    if (IsForwardTiers(i))
                    {
                        float targetAngle = _turnValue * maxSteer;
                        _angle = Mathf.Lerp(_angle, targetAngle, Time.fixedDeltaTime * turnSpeed);
                        tierTransform.localEulerAngles = new Vector3(0f, _angle, 0f);
                    }
                    tireWorldVel = _rb.GetPointVelocity(tierTransform.position);
                    float steeringVel = Vector3.Dot(tierTransform.right, tireWorldVel);
                    float desiredVelChange = -steeringVel * tiers[i].gripFactor;
                    float desireAccel = desiredVelChange / Time.fixedDeltaTime;
                    _rb.AddForceAtPosition(tierTransform.right * tierMass * desireAccel, hit.point);

                    //Accelaration and break
                    if (IsForwardTiers(i))
                    {
                        if (_forwardMove >= 0.1f)
                        {
                            float carSpeed = Vector3.Dot(transform.forward, _rb.velocity);
                            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / maxSpeed);
                            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * _forwardMove * forwardForce;
                            _rb.AddForceAtPosition(tierTransform.forward * availableTorque, hit.point);
                        }
                        else if (_forwardMove <= -0.1f)
                        {
                            _rb.AddForceAtPosition(-tierTransform.forward * breakForce, hit.point);
                        }
                    }

                    // No input Friction
                    if (_forwardMove > -.1f && _forwardMove < .1f)
                    {
                        float carSpeed = Vector3.Dot(transform.forward, _rb.velocity);
                        _rb.AddForceAtPosition(tierTransform.forward * -Mathf.Sign(carSpeed) * regularFrictionForce, hit.point);
                    }
                }
                else
                {
                    _isGrounded = false;
                }
            }

            if (!_isGrounded)
            {
            }
        }

        private bool IsForwardTiers(int i)
        {
            return i == 0 || i == 1;
        }
        private bool IsRearTiers(int i)
        {
            return i == 2 || i == 3;
        }
    }
}