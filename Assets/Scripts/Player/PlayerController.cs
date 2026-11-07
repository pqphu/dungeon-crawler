using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMove();
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
}