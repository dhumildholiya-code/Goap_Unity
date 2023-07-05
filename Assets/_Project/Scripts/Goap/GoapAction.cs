using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public abstract class GoapAction : MonoBehaviour
    {
        private Dictionary<string, Object> _preconditions;
        public Dictionary<string, Object> Preconditions => _preconditions;

        private Dictionary<string, Object> _effects;
        public Dictionary<string, Object> Effects => _effects;

        private bool _inRange = false;
        public bool IsInRange
        {
            get => _inRange;
            set => _inRange = value;
        }

        public float cost = 1f;
        public GameObject target;

        public GoapAction()
        {
            _preconditions = new Dictionary<string, Object>();
            _effects = new Dictionary<string, Object>(); 
        }

        public abstract void Reset();
        public abstract bool IsDone();
        public abstract bool CheckProceduralPrecondition(GameObject agent);
        public abstract bool Perform(GameObject agent);
        public abstract bool RequiresRange();

        public void AddPrecondition(string key, Object value)
        {
            _preconditions.Add(key, value);
        }
        public void RemovePrecondition(string key)
        {
            if(_preconditions.ContainsKey(key))
            {
                _preconditions.Remove(key);
            }
        }
        public void AddEffect(string key, Object value)
        {
            _effects.Add(key, value);
        }
        public void RemoveEffect(string key)
        {
            if (_effects.ContainsKey(key))
            {
                _effects.Remove(key);
            }
        }

        public void DoReset()
        {
            _inRange = false;
            target = null;
            Reset();
        }
    }
}