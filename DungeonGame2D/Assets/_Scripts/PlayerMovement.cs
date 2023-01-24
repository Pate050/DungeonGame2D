using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public float MOVEMENT_BASE_SPEED = 10.0f;

    public float movementSpeed;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private DungeonData dungeonData;
    private Animator anim;
    private SpriteRenderer sprr;

    private Vector2 pointerInput, movementInput;
    public Vector2 PonterInput => pointerInput;

    private Ase ase;
    private bool holdAttack;

    [SerializeField] private InputActionReference movement, attack, pointerPosition;

    private void OnEnable()
    {
        attack.action.canceled += PerformAttack;
        attack.action.performed += HoldAttack;
    }

    private void OnDisable()
    {
        attack.action.canceled -= PerformAttack;
        attack.action.performed -= HoldAttack;
    }

    private void HoldAttack(InputAction.CallbackContext obj)
    {
        if (ase == null)
        {
            Debug.LogError("Weapon parent is null", gameObject);
            return;
        }
        ase.HoldAnAttack();
        holdAttack = true;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        if (ase == null)
        {
            Debug.LogError("Weapon parent is null", gameObject);
            return;
        }
        ase.PerformAnAttack();
        StartCoroutine(Delay());
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        holdAttack = false;
    }

    void Awake(){
        ase = GetComponentInChildren<Ase>();
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
        if (holdAttack)
        {
            Vector2 direction = (pointerInput - (Vector2)transform.position).normalized;
            if (direction.x > 0.1)
            {
                sprr.flipX = false;
            }
            else if (direction.x < -0.1)
            {
                sprr.flipX = true;
            }
        }
        anim.SetFloat("Speed", movementSpeed);
        
    }

    private void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;

        if (holdAttack == false)
        {
            if (movementDirection.x > 0.1)
            {
                sprr.flipX = false;
            }
            else if (movementDirection.x < -0.1)
            {
                sprr.flipX = true;
            }
        }
    }

    void ProcessInputs()
    {
        pointerInput = GetPointerInput();
        ase.PointerPosition = pointerInput;
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        ase.MovementDirection = movementDirection;
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();
    }

    private Vector2 GetPointerInput()
    {
        Vector2 mousePos = pointerPosition.action.ReadValue<Vector2>();
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
