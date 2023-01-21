using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}


public class PlayerMovement : MonoBehaviour
{

    public PlayerState currentState;

    public float MOVEMENT_BASE_SPEED = 10.0f;

    public float movementSpeed;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private DungeonData dungeonData;
    private Animator anim;
    private SpriteRenderer sprr;

    // Start is called before the first frame update
    void Start(){
        currentState = PlayerState.walk;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprr = GetComponentInChildren<SpriteRenderer>();
        if (anim.gameObject.activeSelf)
        {
            anim.Play("CharacterIdleRight");
        }
 
    }

    // Update is called once per frame
    void Update(){
        ProcessInputs();
        Move();
        anim.SetFloat("Speed", movementSpeed);

        bool flipped = movementDirection.x < 0;
        
    }

    private void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
        if (movementDirection.x > 0.1)
        {
            sprr.flipX = false;
        }
        else if (movementDirection.x < -0.1)
        {
            sprr.flipX = true;
        }
    }

    void ProcessInputs()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();
    }
}
