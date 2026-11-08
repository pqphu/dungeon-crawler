using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    //=================== Shoot Arrow ======================
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private int maxAmmo = 20;
    [SerializeField] private float shotDelay = 0.4f;
    private int currentAmmo;
    private float nextShot;
    private Vector2 aimDirection;
    //======================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        HandleMove();
        HandleShoot();
        UpdateAnimation();
    }

    private void HandleMove()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        float horizontal = 0f;
        float vertical = 0f;

        if (kb.aKey.isPressed) horizontal -= 1f;
        if (kb.dKey.isPressed) horizontal += 1f;
        if (kb.sKey.isPressed) vertical -= 1f;
        if (kb.wKey.isPressed) vertical += 1f;

        Vector2 move = new Vector2(horizontal, vertical).normalized;
        rb.linearVelocity = move * moveSpeed;

        if (move.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (move.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateAnimation()
    {
        bool isWalking = rb.linearVelocity.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);
    }

    private void HandleShoot()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Shoot();
           animator.SetTrigger("Attack");

        }
    }
            private void Shoot()
            {
                if (currentAmmo <= 0) return;
                if (Time.time < nextShot) return;

                nextShot = Time.time + shotDelay;

                Vector2 mouseScreen = Mouse.current.position.ReadValue();
                Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
                aimDirection = (mouseWorld - (Vector2)shootPoint.position).normalized;
            }
            public void SpawnArrow()
            {
                if (currentAmmo <= 0) return;
                currentAmmo--;

                GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
                arrow.GetComponent<Arrow>().SetDirection(aimDirection);
            }
}