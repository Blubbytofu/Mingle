using System;
using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header(" ******** Components *********")]
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private Transform orientation;

    [Header(" ******** Gravity ********")]
    [SerializeField] private float gravityMagnitude;
    private Vector3 gravityDir;

    [Header(" ******** Movement ********")]
    [SerializeField] private float strafeForce;
    [SerializeField] private float maxStrafeVelocity;
    [SerializeField] private float airStrafeForceMultiplier;
    private Vector2Int strafeInputDirection;
    private Vector3 strafeRawDirection;

    [Header(" ******** Ground Checking ********")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundSlowDownRate;
    [SerializeField] private float groundZeroSpeedPoint;
    private bool isGrounded;
    private RaycastHit groundHit;

    [Header(" ******** Jumping ********")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldownTime;
    private bool isJumping;

    [Header(" ******** Player States ********")]
    [SerializeField] private PlayerMoveState playerMoveState;

    private void Start()
    {
        strafeInputDirection = new Vector2Int();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJump();
        AdjustMovementPhysics();
        ChangePlayerMoveState();
    }

    public PlayerMoveState GetPlayerMoveState()
    {
        return playerMoveState;
    }

    private void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            strafeInputDirection.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            strafeInputDirection.y = -1;
        }
        else
        {
            strafeInputDirection.y = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            strafeInputDirection.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            strafeInputDirection.x = -1;
        }
        else
        {
            strafeInputDirection.x = 0;
        }

        strafeRawDirection = orientation.right * strafeInputDirection.x + orientation.forward * strafeInputDirection.y;
    }

    private void HandleJump()
    {
        if (!isGrounded || isJumping)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(JumpCooldown());
            playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, 0.0f, playerRB.linearVelocity.z);
            playerRB.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(jumpCooldownTime);
        isJumping = false;
    }

    private void AdjustMovementPhysics()
    {
        // check if player is grounded
        // make movement direction parallel to ground
        isGrounded = Physics.CheckSphere(orientation.position + groundCheckOffset, groundCheckRadius, groundMask);
        if (isGrounded)
        {
            Physics.Raycast(orientation.position, Vector3.down, out groundHit, groundCheckDistance, groundMask);
            strafeRawDirection = Vector3.ProjectOnPlane(strafeRawDirection, groundHit.normal);
        }
        strafeRawDirection.Normalize();

        // change current gravity direction
        if (isGrounded)
        {
            gravityDir = -groundHit.normal;
        }
        else
        {
            gravityDir = Vector3.down;
        }

        // bind upper movement speed
        if (isGrounded)
        {
            if (!isJumping)
            {
                Vector3 slopeAdjustedMoveVelocity = Vector3.ProjectOnPlane(playerRB.linearVelocity, groundHit.normal);
                if (slopeAdjustedMoveVelocity.magnitude >= maxStrafeVelocity)
                {
                    slopeAdjustedMoveVelocity.Normalize();
                    playerRB.linearVelocity = maxStrafeVelocity * slopeAdjustedMoveVelocity;
                }
            }
        }
        else
        {
            Vector3 horizontalAirVelocity = new Vector3(playerRB.linearVelocity.x, 0.0f, playerRB.linearVelocity.z);
            if (horizontalAirVelocity.magnitude >= maxStrafeVelocity)
            {
                horizontalAirVelocity.Normalize();
                Vector3 targetVelocity = maxStrafeVelocity * horizontalAirVelocity;
                playerRB.linearVelocity = new Vector3(targetVelocity.x, playerRB.linearVelocity.y, targetVelocity.z);
            }
        }

        // reduce no input speed
        if (isGrounded)
        {
            if (strafeRawDirection.Equals(Vector3.zero))
            {
                if (playerRB.linearVelocity.magnitude > 0)
                {
                    playerRB.linearVelocity = Vector3.Lerp(playerRB.linearVelocity, Vector3.zero, groundSlowDownRate * Time.deltaTime);
                    if (playerRB.linearVelocity.magnitude < groundZeroSpeedPoint)
                    {
                        playerRB.linearVelocity = Vector3.zero;
                    }
                }
            }
        }
    }

    private void ChangePlayerMoveState()
    {
        if (!isGrounded)
        {
            playerMoveState = PlayerMoveState.AIRBORNE;
            return;
        }

        if (strafeInputDirection.Equals(Vector2Int.zero))
        {
            playerMoveState = PlayerMoveState.STILL;
        }
        else
        {
            playerMoveState = PlayerMoveState.RUNNING;
        }
    }

    private void FixedUpdate()
    {
        ApplyStrafe();
        ApplyCustomGravity();
    }

    private void ApplyStrafe()
    {
        Vector3 force = strafeForce * strafeRawDirection;
        if (!isGrounded)
        {
            force *= airStrafeForceMultiplier;
        }
        playerRB.AddForce(force, ForceMode.Acceleration);
    }

    private void ApplyCustomGravity()
    {
        playerRB.AddForce(gravityMagnitude * gravityDir, ForceMode.Acceleration);
    }
}
