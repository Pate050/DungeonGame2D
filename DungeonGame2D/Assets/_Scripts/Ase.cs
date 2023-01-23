using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ase : MonoBehaviour
{
    public SpriteRenderer characterRenderer, weaponRenderer;
    public Vector2 PointerPosition { get; set; }

    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;
    private Vector2 direction;
    private SpriteRenderer slash;

    private void Awake()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
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
            transform.right = direction;
        }
        Vector2 scale = transform.localScale;
        if(direction.x < 0)
        {
            scale.y = -1;
        }else if(direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
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
        yield return new WaitForSeconds(0.1f);
        if (slash.enabled)
        { 
            slash.enabled = false; 
        }
        else
        {
            slash.enabled = true;
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
