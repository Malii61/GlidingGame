using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float bouncyAmount; // Amount of bounce to apply on collision

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Rigidbody rb))
        {
            BallController.Instance.ChangeShape(BallController.Shape.Ball);
            // Get the point of collision and the collision surface's normal
            ContactPoint contactPoint = collision.contacts[0];
            Vector3 contactNormal = contactPoint.normal;

            // Reverse the movement direction and apply force to the object
            float forceMagnitude = rb.velocity.magnitude; // Magnitude of force based on the object's velocity
            Vector3 oppositeForce = -contactNormal * forceMagnitude * bouncyAmount;
            Debug.Log(oppositeForce.y);

            // Clamp the vertical force to ensure controlled bouncing
            oppositeForce.y = Mathf.Clamp(oppositeForce.y, 20f, 80f);

            rb.AddForce(oppositeForce, ForceMode.Impulse);
        }
    }
}
