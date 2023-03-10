using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int currentHealth, maxHealth;
    public bool canDie = true;
    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;
    [SerializeField] private bool isDead = false;

    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    public void GetHit(int amount, GameObject sender)
    {
        if (canDie == false)
            return;
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;
        SimpleFlash flash;
        if (flash = GetComponentInChildren<SimpleFlash>())
        {
            flash.Flash();
        }
        currentHealth -= amount;

        if (currentHealth > 0)
        {
            OnHitWithReference.Invoke(sender);
        }
        else
        {
            OnDeathWithReference.Invoke(sender);
            isDead = true;
            Animator anim;
            Collider2D coll;
            if (coll = GetComponent<Collider2D>())
            {
                coll.enabled = false;
            }
            if (anim = GetComponent<Animator>())
            {
                anim.SetBool("Smash", true);
                StartCoroutine(Delay());
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }

}
