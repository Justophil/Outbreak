using UnityEngine;

namespace Player
{
    public static class GroundCheck
    {
        public static bool IsGrounded(Transform transform, float groundDistance, LayerMask groundLayer)
        {
            RaycastHit hit;
            bool grounded = Physics.Raycast(
                transform.position + new Vector3(0, 1f, 0),
                Vector3.down,
                out hit,
                groundDistance,
                groundLayer
            );
            return grounded;
        }
    }

}