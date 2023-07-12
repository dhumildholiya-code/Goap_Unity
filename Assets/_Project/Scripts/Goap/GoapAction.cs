using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public abstract class GoapAction : MonoBehaviour
    {
        public float cost = 1f;
        public GameObject target = null;

        private Dictionary<string, object> _preCondition;
        public Dictionary<string, object> PreCondition => _preCondition;
        private Dictionary<string, object> _effects;
        public Dictionary<string, object> Effects => _effects;

        private bool _inRange = false;
        public bool InRange
        {
            get => _inRange;
            set => _inRange = value;
        }

        public abstract void Reset();
        public abstract bool IsDone();
        public abstract bool CheckProceduralPreCondition(GameObject agent);
        public abstract bool RequiresInRange();

        public void DoReset()
        {
            _inRange = false;
            target = null;
            Reset();
        }

        public void AddPreCondition(string key, object value)
        {
            if (!_preCondition.ContainsKey(key))
            {
                _preCondition.Add(key, value);
            }
        }
        public void RemovePreCondition(string key)
        {
            if (_preCondition.ContainsKey(key))
            {
                _preCondition.Remove(key);
            }
        }

        public void AddEffect(string key, object value)
        {
            if (!_effects.ContainsKey(key))
            {
                _effects.Add(key, value);
            }
        }
        public void RemoveEffect(string key)
        {
            if (_effects.ContainsKey(key))
            {
                _effects.Remove(key);
            }
        }
    }
}