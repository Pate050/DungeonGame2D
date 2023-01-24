using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ase : MonoBehaviour
{
    public SpriteRenderer characterRenderer, weaponRenderer;
    public Vector2 PointerPosition { get; set; }
    public Vector2 MovementDirection { get; set; }

    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;
    private Vector2 direction;
    private SpriteRenderer slash;
    private bool hold = false;
    private Collider2D hitBox;

    private void Awake()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        hitBox = GetComponentInChildren<Collider2D>();
        foreach (SpriteRenderer sprite in sprites)
        {
            if (sprite.gameObject.name == "Slash")
                slash = sprite;
        }
    }
    private void Update()
    {
        if (slash.enabled == false)
        {
            direction = (PointerPosition - (Vector2)transform.position).normalized;
            Vector2 scale = transform.localScale;
            if (hold)
            {                
                transform.right = direction;      
                if (direction.x < 0)
                {
                    scale.y = -1;
                }
                else if (direction.x > 0)
                {
                    scale.y = 1;
                }                
            }
            else
            {
                if (MovementDirection.x > 0.1)
                {
                    transform.right = Vector2.right + 0.2f * Vector2.down;
                    scale.y = 1;
                }
                else if (MovementDirection.x < -0.1)
                {
                    transform.right = Vector2.left + 0.2f * Vector2.down;
                    scale.y = -1;
                }
            }
            transform.localScale = scale;

            if (transform.eulerAngles.z > 330 || transform.eulerAngles.z < 200)
            {
                weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
            }
            else
            {
                weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
            }
        }
        
    }

    public void HoldAnAttack()
    {
        hold = true;
    }

    public void PerformAnAttack()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attack");
        StartCoroutine(Slash());
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }
    private IEnumerator Slash()
    {
        yield return new WaitForSeconds(0.05f);
        if (slash.enabled)
        { 
            slash.enabled = false;
            hitBox.enabled = false;
            hold = false;
        }
        else
        {
            slash.enabled = true;
            hitBox.enabled = true;
            StartCoroutine(Slash());
        }
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
        slash.enabled = false;
    }


}
