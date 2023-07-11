using TMPro;
using UnityEngine;

namespace CarPhysics
{
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
        [SerializeField] private Transform[] tiers;
        public float tierMass;
        [Range(0f, 1f)] public float tierGripFactor;
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
                Transform tier = tiers[i];


                Debug.DrawRay(tier.position, -tier.up * restLength, Color.red);
                if (Physics.Raycast(tier.position, -tier.up, out RaycastHit hit, restLength))
                {
                    _isGrounded = true;
                    Debug.DrawLine(tier.position, hit.point, Color.green);

                    // Suspension
                    Vector3 tireWorldVel = _rb.GetPointVelocity(tier.position);
                    float offset = restLength - hit.distance;
                    float vel = Vector3.Dot(tier.up, tireWorldVel);
                    float force = (offset * springStrength) - (vel * springDamper);
                    _rb.AddForceAtPosition(tier.up * force, hit.point);

                    //Steering
                    // apply rotation on forward tiers.
                    if (IsForwardTiers(i))
                    {
                        float targetAngle = _turnValue * maxSteer;
                        _angle = Mathf.Lerp(_angle, targetAngle, Time.fixedDeltaTime * turnSpeed);
                        tier.localEulerAngles = new Vector3(0f, _angle, 0f);
                    }
                    tireWorldVel = _rb.GetPointVelocity(tier.position);
                    float steeringVel = Vector3.Dot(tier.right, tireWorldVel);
                    float desiredVelChange = -steeringVel * tierGripFactor;
                    float desireAccel = desiredVelChange / Time.fixedDeltaTime;
                    _rb.AddForceAtPosition(tier.right * tierMass * desireAccel, hit.point);

                    //Accelaration and break
                    if (IsForwardTiers(i))
                    {
                        if (_forwardMove >= 0.1f)
                        {
                            float carSpeed = Vector3.Dot(transform.forward, _rb.velocity);
                            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / maxSpeed);
                            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * _forwardMove * forwardForce;
                            _rb.AddForceAtPosition(tier.forward * availableTorque, hit.point);
                        }
                        else if (_forwardMove <= -0.1f)
                        {
                            _rb.AddForceAtPosition(-tier.forward * breakForce, hit.point);
                        }
                    }

                    // No input Friction
                    if (_forwardMove > -.1f && _forwardMove < .1f)
                    {
                        float carSpeed = Vector3.Dot(transform.forward, _rb.velocity);
                        _rb.AddForceAtPosition(tier.forward * -Mathf.Sign(carSpeed) * regularFrictionForce, hit.point);
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