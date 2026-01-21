using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float ricochetAngleThreshold = 30f; // Maximum angle (degrees) for ricochet
    public float ricochetDamping = 0.8f; // Speed retention factor after ricochet
    public float lifeTime = 5f; // Auto-destroy after some time

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Bullet requires a Rigidbody.");
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, lifeTime); // Destroy if it doesn't hit anything after a while
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0]; // Get first contact point
        Vector3 incomingDirection = rb.velocity.normalized; // Bullet's movement direction
        Vector3 normal = contact.normal; // Surface normal

        float impactAngle = Vector3.Angle(incomingDirection, -normal); // Compute impact angle

        if (impactAngle <= ricochetAngleThreshold)
        {
            // Ricochet logic
            Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, normal); // Compute bounce direction
            rb.velocity = reflectedDirection * rb.velocity.magnitude * ricochetDamping; // Keep original speed factor

            transform.rotation = Quaternion.LookRotation(reflectedDirection); // Rotate bullet to new direction
        }
        else 
        {
            Destroy(gameObject); // Delete bullet if angle is too steep
        }
    }
}
