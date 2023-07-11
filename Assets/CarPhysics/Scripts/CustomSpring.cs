using UnityEngine;

namespace CarPhysics
{
    public class CustomSpring
    {
        private const float stepSize = 1f / 60f;

        private float _position;
        private float _goal;
        private float _velocity;
        private float _stiffness;
        private float _damping;

        public CustomSpring(float stiffness, float damping)
        {
            _position = 0f;
            _velocity = 0f;
            _goal = 0f;
            _stiffness = stiffness;
            _damping = damping;
        }

        /// <summary>
        /// Set value where you want to start your simulation.
        /// </summary>
        /// <param name="value"></param>
        public void SetPos(float value)
        {
            _position = value;
        }
        public void SetGoal(float goal)
        {
            _goal = goal;
        }

        /// <summary>
        /// Return float between value you have passed in SetPos and 0f.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public float Evaluate(float deltaTime)
        {
            CalcDampedSimpleHarmonicMotion(_goal, deltaTime, _stiffness, _damping);
            return _position;
        }

        private void CalcDampedSimpleHarmonicMotion(
            float equilibriumPosition,
            float deltaTime,
            float stiffness,
            float damping
            )
        {
            float x = _position;
            float v = _velocity;

            float steps = Mathf.Ceil(deltaTime / stepSize);
            for (var i = 0; i < steps; i++)
            {
                var dt = i == steps - 1 ? deltaTime - i * stepSize : stepSize;

                float m = 1f;
                var a = (-stiffness * (x - equilibriumPosition) + -damping * v) / m;
                v += a * dt;
                x += v * dt;
            }

            _position = x;
            _velocity = v;
        }
    }
}