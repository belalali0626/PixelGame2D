using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("Components Variables")]
    private Rigidbody2D rb;

    [Header("Layer Masks")]
    [SerializeField]private LayerMask groundLayer;

    [Header("Movement Variables")]
    [SerializeField]private float movementAcceleration;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float linearDrag;
    private float horizontalDirection;
    private bool changingDirection => (rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f);

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 8f;
    [SerializeField] private float lowJumpFallMultiplier = 5f;
    [SerializeField] private int extrajump = 1;
    private int extraJumpValue;

    private float airLinearDrag = 2.5f;
    private bool canJump => Input.GetButtonDown("Jump") && (onGround || extraJumpValue > 0);

    [Header("Collison Variables")]
    [SerializeField] private float groundRayCastLength;
    [SerializeField]private bool onGround;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalDirection = GetInput().x;
        if (canJump) Jump();

    }
    private void FixedUpdate()
    {
        CheckCollsion();
        MoveCharacter();
        if (onGround)
        {
            extraJumpValue = extrajump;
            ApplyLinearDrag();
        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
        }
    }
    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void MoveCharacter()
    {
        rb.AddForce(new Vector2(horizontalDirection, 0) * movementAcceleration);
        if(Mathf.Abs(rb.velocity.x) > maxMoveSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
        }
    }
    private void ApplyLinearDrag()
    {
        if(Mathf.Abs(horizontalDirection) < 0.4f || changingDirection)
        {
            rb.drag = linearDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }
    private void ApplyAirLinearDrag()
    {
            rb.drag = airLinearDrag;
    }

    private void Jump()
    {
        if(!onGround)
        {
            extraJumpValue--;
        }
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }    
    private void CheckCollsion()
    {
        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundRayCastLength, groundLayer);
    }
    private void FallMultiplier()
    {
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRayCastLength);
    }
}
