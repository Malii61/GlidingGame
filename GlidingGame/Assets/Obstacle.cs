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
            // �arp��man�n ger�ekle�ti�i noktay� ve �arp��man�n y�zey normalini al
            ContactPoint contactPoint = collision.contacts[0];
            Vector3 contactNormal = contactPoint.normal;

            // Hareket y�n�n� tersine �evirerek nesneye kuvvet uygula
            float forceMagnitude = rb.velocity.magnitude; // Kuvvet b�y�kl���, nesnenin h�z�na ba�l� olarak
            Vector3 oppositeForce = -contactNormal * forceMagnitude * bouncyAmount;

            rb.AddForce(oppositeForce, ForceMode.Impulse);
  
        }
    }
}