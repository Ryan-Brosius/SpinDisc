using UnityEngine;

namespace Paper.Core.Spring
{
    [System.Serializable]
    public struct Spring
    {
        public float halfLife;
        public float frequency;

        private Vector3 _position;
        private Vector3 _velocity;

        public Vector3 Position => _position;

        public Spring(float halfLife = 0.075f, float frequency = 18f)
        {
            this.halfLife = halfLife;
            this.frequency = frequency;
            _position = Vector3.zero;
            _velocity = Vector3.zero;
        }

        public void Initialize(Vector3 position)
        {
            _position = position;
            _velocity = Vector3.zero;
        }

        public Vector3 Update(Vector3 target, float deltaTime)
        {
            var dampingRatio = -Mathf.Log(0.5f) / (frequency / halfLife);
            var f = 1.0f + 2.0f * deltaTime * dampingRatio * frequency;
            var oo = frequency * frequency;
            var hoo = deltaTime * oo;
            var hhoo = deltaTime * hoo;
            var detInv = 1.0f / (f + hhoo);
            var detX = f * _position + deltaTime * _velocity + hhoo * target;
            var detV = _velocity + hoo * (target - _position);
            _position = detX * detInv;
            _velocity = detV * detInv;
            return _position;
        }
    }
}
