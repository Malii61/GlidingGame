using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float bouncyAmount;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Rigidbody rb))
        {
            BallController.Instance.ChangeShape(BallController.Shape.Ball);
            // Çarpýþmanýn gerçekleþtiði noktayý ve çarpýþmanýn yüzey normalini al
            ContactPoint contactPoint = collision.contacts[0];
            Vector3 contactNormal = contactPoint.normal;

            // Hareket yönünü tersine çevirerek nesneye kuvvet uygula
            float forceMagnitude = rb.velocity.magnitude; // Kuvvet büyüklüðü, nesnenin hýzýna baðlý olarak
            Vector3 oppositeForce = -contactNormal * forceMagnitude * bouncyAmount;

            rb.AddForce(oppositeForce, ForceMode.Impulse);
  
        }
    }
}