using UnityEngine;

namespace CarPhysics
{
    public class CarController : MonoBehaviour
    {
        [Header("Tires")]
        [SerializeField] private Transform[] tiers;
        public float tierMass;
        [Range(0f, 1f)] public float tierGripFactor;
        [Header("Suspension")]
        public float restLength = 1f;
        public float springTravel = .5f;
        public float springStrength = 30000f;
        public float springDamper = 4000f;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
        }

        private void FixedUpdate()
        {
            float maxLength = restLength + springTravel;
            for (int i = 0; i < tiers.Length; i++)
            {
                Transform tier = tiers[i];
                Debug.DrawRay(tier.position, -tier.up * maxLength, Color.red);
                if (Physics.Raycast(tier.position, -tier.up, out RaycastHit hit, maxLength))
                {
                    Debug.DrawLine(tier.position, hit.point, Color.green);

                    // Suspension
                    Vector3 tireWorldVel = _rb.GetPointVelocity(tier.position);
                    float offset = restLength - hit.distance;
                    float vel = Vector3.Dot(tier.up, tireWorldVel);
                    float force = (offset * springStrength) - (vel * springDamper);
                    _rb.AddForceAtPosition(tier.up * force, tier.position);

                    //Steering
                    tireWorldVel = _rb.GetPointVelocity(tier.position);
                    float steeringVel = Vector3.Dot(tier.right, tireWorldVel);
                    float desiredVelChange = -steeringVel * tierGripFactor;
                    float desireAccel = desiredVelChange / Time.fixedDeltaTime;
                    _rb.AddForceAtPosition(tier.right * tierMass * desireAccel, tier.position);

                    if (i == 2 || i == 3)
                    {
                    }
                }
            }
        }
    }
}