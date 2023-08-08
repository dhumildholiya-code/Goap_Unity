using UnityEngine;

namespace CarPhysics
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float rayLength;
        [SerializeField] private float restHeight;
        [SerializeField] private float springStrength;
        [SerializeField] private float springDamper;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * rayLength, Color.red);
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                float relVel = Vector3.Dot(ray.direction, _rb.velocity);
                float x = hit.distance - restHeight;
                float springForce = (x * springStrength) - (relVel * springDamper);
                _rb.AddForce(ray.direction * springForce);
            }
        }

    }
}