using UnityEngine;

namespace Obi.Samples
{
    public class RopeTenser : MonoBehaviour
    {
        public float force = 10;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            _rb?.AddForce(Vector3.down * force);
        }
    }
}
