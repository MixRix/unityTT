using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
        {
        if (collision.gameObject.CompareTag("Player"))
        {
         ThirdPersonShooterController playerHealth = collision.gameObject.GetComponent<ThirdPersonShooterController>();
         playerHealth.TakeDamage(10);
        }
        Destroy(gameObject);
        }
    
}
