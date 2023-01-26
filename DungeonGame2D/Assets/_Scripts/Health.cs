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

        currentHealth -= amount;

        if (currentHealth > 0)
        {
            OnHitWithReference.Invoke(sender);
        }
        else
        {
            OnDeathWithReference.Invoke(sender);
            isDead = true;
            Animator anim = new Animator();
            if (anim = GetComponent<Animator>())
            {
                Debug.Log("AnimatorFound");
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
