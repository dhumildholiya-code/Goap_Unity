using UnityEngine;

namespace CarPhysics
{
    public class VisualTier : MonoBehaviour
    {
        [SerializeField] private Transform tier;

        private float _offset;
        private void Start()
        {
            _offset = transform.localEulerAngles.y;
        }

        private void Update()
        {
            transform.rotation = Quaternion.Euler(0f, _offset + tier.localEulerAngles.y, 0f);
        }
    }
}