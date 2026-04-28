using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement_2D : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float speed = 6f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 3f;
    public float jumpForce = 8f;
    float currentSpeed;

    [Header("Sprint")]
    bool sprinting;

    [Header("Crouch")]
    bool crouching;
    public float crouchHeight = 0f;
    private bool displayCrouchSprite;

    [Header("jump")]
    float jumpInput;
    private float wallJumpTimer = 0f;

    [Header("Wall Jump")]
    public float wallJumpForce = 18f;
    public float wallJumpHeightBias = 1.5f;
    public float wallJumpLockoutTime = 0.2f;
    public LayerMask groundAndWallAndCeilingLayer = -1;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;

    [Header("Wall Detection")]
    public float skinWidth = 0.02f;
    bool leftWall;
    bool rightWall;
    bool isWalled;
    bool isWalledandGround;

    [Header("Ground Detection")]
    float groundCheckDistance;
    private bool isGrounded;

    [Header("Ceiling Detection")]
    private bool isCeilinged;

    [Header("Sprites - Drag Your Sprites Here")] /* Left in the arrows, just for bug testing. Also placed check mark comments to make sure I put I had them implemented */
    public Sprite blueArrow;         //Regular/Idle/Move/Fall
    public Sprite greenArrow;        //Crouch
    public Sprite yellowDoubleArrow; //Sprint (while moving)
    public Sprite redArrow;          //Jumping (upward velocity)
    public Sprite pinkArrow;         //On Wall slide
    public Sprite idleAnim; //Check
    public Sprite walkingAnim; //Check
    public Sprite crouchingIdleAnim; //Check
    public Sprite crouchWalkingAnim; //Check
    public Sprite sprintIdleAnim; //Check
    public Sprite sprintingAnim; //Check
    public Sprite jumpingAnim; //Check
    public Sprite fallingAnim; //Cheack
    public Sprite wallingAnim; //Check
    public Sprite pushingAnim; //
    public Sprite interactingAnim; //

    private Rigidbody rb;
    private CapsuleCollider cc;
    private SpriteRenderer sr; //Sprite Square's renderer

    private float standHeight;
    private Vector3 standCenter;
    private float radius;
    private float inputCache; //For sprint checking

    //Animation
    private Animator animator;
    private GameObject ChildObject;

    // Updated/Fixed for moving platforms
    private Transform currentPlatform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        sr = GetComponentInChildren<SpriteRenderer>(); //Autograbs the Sprite Square in the blueprint
        ChildObject = GameObject.Find("PlayerSquare");
        animator = ChildObject.GetComponent<Animator>();

        standHeight = cc.height;
        standCenter = cc.center;
        radius = cc.radius;
        if (sr == null) Debug.LogError("No SpriteRenderer on child Square, fix that");
        UpdateSprite();  // Start with blue
    }

    void Update()
    {
        //Assign Circumstances
        AssignGenericCircumstances();

        //Handle charactor movement
        HandleSprinting();
        HandleCrouching();
        HandleJumping();

        //Swap sprites by movement
        UpdateSprite();
    }

    void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
    }

    void AssignGenericCircumstances() //Aimed at direct input
    {
        //Input assignment
        sprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = speed;

        crouching = Input.GetKey(KeyCode.LeftControl);

        jumpInput = Input.GetAxisRaw("Horizontal");
        inputCache = jumpInput;

        /* Checker notes
        //We would want to find a way to make these checkers work in FixedUpdate to reduce CPU usage on Physics
        //Current problem with this: leads to edge cases / jank, and I'm struggling to fix it
        //Chat GPT taught me what a Sphere cast and Capsule cast are and how they work.
        //It generated this code, and the wall checker code as a raycast alternative. I made a minor change.
        */

        //Ground checker
        groundCheckDistance = 1.01f;

        isGrounded = Physics.SphereCast(
            transform.position,
            radius * 0.3f,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundAndWallAndCeilingLayer,
            QueryTriggerInteraction.Ignore
        );

        // Updated/Fixed for moving platforms
        // Detect if we are standing on a moving platform
        if (isGrounded && hit.collider != null)
        {
            currentPlatform = hit.collider.transform;
        }
        else if (!isGrounded)
        {
            currentPlatform = null;
        }

        //Ceiling checker
        isCeilinged = Physics.Raycast(transform.position, Vector3.up, 1.01f, groundAndWallAndCeilingLayer, QueryTriggerInteraction.Ignore);

        //Wall Checker
        float wallCheckDistance = 0.1f;

        leftWall = Physics.CapsuleCast(
            cc.bounds.center + Vector3.up * (cc.height / 2 - radius),
            cc.bounds.center - Vector3.up * (cc.height / 2 - radius),
            radius * 0.9f,
            Vector3.left,
            wallCheckDistance,
            groundAndWallAndCeilingLayer,
            QueryTriggerInteraction.Ignore
        );

        rightWall = Physics.CapsuleCast(
            cc.bounds.center + Vector3.up * (cc.height / 2 - radius),
            cc.bounds.center - Vector3.up * (cc.height / 2 - radius),
            radius * 0.9f,
            Vector3.right,
            wallCheckDistance,
            groundAndWallAndCeilingLayer,
            QueryTriggerInteraction.Ignore
        );
        isWalled = !isGrounded && ((leftWall && jumpInput < 0f) || (rightWall && jumpInput > 0f));

        //Wall and ground simeotanious checker
        isWalledandGround = isGrounded && ((leftWall && jumpInput < 0f) || (rightWall && jumpInput > 0f));
    }

    void HandleSprinting() //Handles sprinting
    {
        if (sprinting)
        {
            currentSpeed = sprintSpeed;
        }
    }

    void HandleCrouching() //Dynamic crouching
    {
        if (crouching && isGrounded) //Trying to crouch
        {
            if (!isCeilinged) //Not under ceiling
            {
                displayCrouchSprite = true;
                //Debug.Log("Crouching in General!");
                cc.height = crouchHeight;
                cc.center = new Vector3(standCenter.x, (standCenter.y - (standHeight - crouchHeight) / 2f) + 0.01f, standCenter.z);
                //-0.01f stops edge cases where the character is stuck in the ceiling
                if (isGrounded)
                {
                    currentSpeed = crouchSpeed;
                }
            }
            else //Under ceiling
            {
                displayCrouchSprite = true;
                //Debug.Log("Crouching Under Something!");
                cc.height = crouchHeight;
                cc.center = new Vector3(standCenter.x, (standCenter.y - (standHeight - crouchHeight) / 2f) + 0.01f, standCenter.z);
                //-0.01f stops edge cases where the character is stuck in the ceiling
                currentSpeed = crouchSpeed;
            }
        }
        else //Not trying to crouch
        {
            if (!isCeilinged) //Not Under ceiling
            {
                displayCrouchSprite = false;
                //Debug.Log("Standing Normally");
                cc.height = standHeight;
                cc.center = standCenter;
            }
            else //Under ceiling
            {
                if (isGrounded)
                {
                    displayCrouchSprite = true;
                    //Debug.Log("Stuck Crouching");
                    cc.height = crouchHeight;
                    cc.center = new Vector3(standCenter.x, (standCenter.y - (standHeight - crouchHeight) / 2f) + 0.01f, standCenter.z);
                    //-0.01f stops edge cases where the character is stuck in the ceiling
                    currentSpeed = crouchSpeed;
                }
            }
        }
    }

    void HandleJumping() //Dynamic jumping
    {
        /* Original code, was replaced by sphere casting in AssignGenericCircumstancesFixedUpdate. Leaves bad edge cases but is more lightweight
        //WALL RAYS: From edges, outward, NO SELF-HITS
        //Vector3 baseOrigin = transform.position + Vector3.down * 0.5f; // Mid-body
        //Vector3 leftOrigin = baseOrigin - Vector3.right * (radius - skinWidth);
        //Vector3 rightOrigin = baseOrigin + Vector3.right * (radius - skinWidth);
        //bool leftWall = Physics.Raycast(leftOrigin, Vector3.left, skinWidth * 2f, groundAndWallAndCeilingLayer);
        //bool rightWall = Physics.Raycast(rightOrigin, Vector3.right, skinWidth * 2f, groundAndWallAndCeilingLayer);
        */

        //Jump
        if (isGrounded && !isWalled && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //Debug.Log("Jump!");
        }

        //Corner Jump
        if (isWalledandGround && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //Debug.Log("Corner Jump!");
        }

        //Wall jump: Diagonal Direct Velocity
        if (!isGrounded && isWalled && Input.GetButtonDown("Jump"))
        {
            Vector3 dir = new Vector3(-jumpInput, wallJumpHeightBias, 0f).normalized;
            Vector3 kick = dir * wallJumpForce;
            rb.velocity = new Vector3(kick.x, rb.velocity.y + kick.y, 0f);
            wallJumpTimer = wallJumpLockoutTime;
            //Debug.Log("Wall Jump!");
        }

        //Timer tick for wall jump
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer < 0f) wallJumpTimer = 0f;
        }

        // H-Velocity: Slide on wall-hold
        float targetHVel = jumpInput * currentSpeed;
        if (!isGrounded && isWalled && wallJumpTimer <= 0f)
        {
            targetHVel = 0f; //WALL SLIDE: Gravity down, no push-in
        }

        float finalHVel = (wallJumpTimer > 0f) ? rb.velocity.x : targetHVel;
        float finalYVel = rb.velocity.y;

        //Slow down the character if wall sliding (ChatGPT gave me this code and taught me how/why it works)
        if (!isGrounded && isWalled && wallJumpTimer <= 0f)
        {
            finalYVel = Mathf.Max(rb.velocity.y, -wallSlideSpeed);
        }

        // Updated/Fixed for moving platforms
        // Platform velocity inheritance + player input
        // Allows the player to walk left/right on top of the moving platform
        if (isGrounded && currentPlatform != null && currentPlatform.GetComponent<MovingPlatform>() != null)
        {
            Vector3 platformVelocity = currentPlatform.GetComponent<Rigidbody>().velocity;
            finalHVel = platformVelocity.x + (jumpInput * currentSpeed);
        }
        finalHVel = Mathf.Clamp(finalHVel, -currentSpeed * 1.5f, currentSpeed * 1.5f);

        //Final calculated velocity
        rb.velocity = new Vector3(finalHVel, finalYVel, 0f);

        //Flip
        if (jumpInput != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(jumpInput), transform.localScale.y, transform.localScale.z);
        }
    }

    void UpdateSprite() //Updates the sprites accordingly
    {

        //Used for Debugging on 3/24/26. Can be removed
        if (animator.GetBool("isInteracting"))
        {
            Debug.Log("interacting");
        }

        //Crouching Sprites/Anims
        if (displayCrouchSprite && Mathf.Abs(inputCache) > 0f)
        {
            //sr.sprite = crouchWalkingAnim;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", true);
            animator.SetBool("isInteracting", false);
            return;
        }
        if (displayCrouchSprite && !(Mathf.Abs(inputCache) > 0f))
        {
            //sr.sprite = crouchingIdleAnim;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isCrouching", true);
            return;
        }
        //Walling Sprites/Anims
        if (isWalled)
        {
            //sr.sprite = wallingAnim;
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isInteracting", false);
            return;
        }
        //Jumping/Falling Sprites/Anims
        if (!isGrounded && rb.velocity.y > 0.1f) //Jumping
        {
            //sr.sprite = jumpingAnim;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isJumping", true);
            return;
        }
        if (!isGrounded && rb.velocity.y < 0.1f) //Falling I think this will work???
        {
            //sr.sprite = fallingAnim;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isFalling", true);
            return;
        }
        //Sprinting Sprites/Anims
        if (sprinting && !(Mathf.Abs(inputCache) > 0f)) //Sprinting while not moving: Sprint Idle
        {
            //sr.sprite = sprintIdleAnim;
            animator.SetBool("isWalking", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isIdle", true);
            return;
        }
        if (sprinting && Mathf.Abs(inputCache) > 0f) //Sprinting while moving
        {
            //sr.sprite = sprintingAnim;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isRunning", true);
            return;
        }

        //Insert Pushing Sprites/Anims Code
        //Walking Sprites/Animes
        if (Mathf.Abs(inputCache) > 0f)
        {
            //sr.sprite = walkingAnim;
            animator.SetBool("isIdle", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isWalking", true);
            return;
        }
        //sr.sprite = idleAnim; //Default: Regular/Idle/Move/Fall (blank ignored)
        animator.SetBool("isWalking", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isCrouching", false);
        animator.SetBool("isInteracting", false);
        animator.SetBool("isIdle", true);

        //Insert Interact Sprites/Anims Code

    }

    void OnDrawGizmosSelected()
    {
        // Ground
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.01f);

        // Wall rays (cyan/magenta for sides)
        Vector3 baseOrigin = transform.position + Vector3.down * 0.5f;
        float skin = skinWidth;
        Vector3 leftOrigin = baseOrigin - Vector3.right * (radius - skin);
        Vector3 rightOrigin = baseOrigin + Vector3.right * (radius - skin);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(leftOrigin, Vector3.left * skin * 2f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(rightOrigin, Vector3.right * skin * 2f);
    }
}