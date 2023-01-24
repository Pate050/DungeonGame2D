using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Health health;
        if(health = collider.GetComponent<Health>())
        {
            health.GetHit(damage, transform.parent.gameObject);
        }
    }
}
