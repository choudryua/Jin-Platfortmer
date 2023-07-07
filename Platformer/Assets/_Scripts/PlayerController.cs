using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform _wallCheckPt;
    [SerializeField] private Transform _LeftwallCheckPt;
    [SerializeField] private Vector2 _wallCheckSz;
    [SerializeField] private float _attackSz;
    [SerializeField] private GameObject _attackPt;

    [SerializeField] private float DashingPower;
    [SerializeField] private float DashingCD;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing = true;
    [SerializeField] private float dashingTime;
    [SerializeField] private bool isDead = false;
    [SerializeField] private AudioClip _Dashclip;




    TrailRenderer tr;

    public float LastPressedJumpTime { get; private set; }
    private float originalGravity;
    public ParticleSystem dust;
    public ParticleSystem expo;
    public ParticleSystem expoC;
    public ParticleSystem expoD;
    private bool grounded;
    Vector2 playerMovement;
    Rigidbody2D myRigidBody;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    public LayerMask enemies;
    public Animator anim;
    bool facingRight;
    SpriteRenderer _sprite;
    Vector3 CurrentPosition;
    public bool isJumping = false;
    Vector2 startPosition;
    AnimationClip die;
    Animator animator;
    private Color oldColor;




    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        originalGravity = myRigidBody.gravityScale;
        tr = GetComponent<TrailRenderer>();
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        oldColor = _sprite.color;
        
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        #region TIMERS
        
        LastPressedJumpTime -= Time.deltaTime;

        #endregion
        Run();
        Flip();
        //limitSpeed();
        JumpAnim();
    }



    private void OnDash(InputValue value)
    {
        if (value.isPressed && canDash == true && isDead == false)
        {
            StartCoroutine(Dash());
            print("DASH U FUCKER");
            SoundManager.instance.PlaySound(_Dashclip);
        }
 
    }

    void OnMove(InputValue value)
    {
        playerMovement = value.Get<Vector2>();


        if (IsGrounded())
        {
            CreateDust();
        }


        print(playerMovement);


        if (playerMovement.x != 0 && IsGrounded())
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    
    void Run()
    {
        if (isDashing == false && isJumping == false && isDead == false)
        {
            myRigidBody.velocity = new Vector2(playerMovement.x * speed , myRigidBody.velocity.y); 
        }
    }

    private void OnJump(InputValue value)
    {
        StartCoroutine(Jump());
    }

    public bool IsWallR()
    {

        if (Physics2D.OverlapBox(_wallCheckPt.position, _wallCheckSz, 0, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWallL()
    {
        if (Physics2D.OverlapBox(_LeftwallCheckPt.position, _wallCheckSz, 0, groundLayer))
        {
            canDash = true;
            return true;
        } else { return false;}
    }
    

    public bool IsGrounded()
    {
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            canDash = true;
            return true;
        }
        else
        {

            return false;
        }
     }



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(_wallCheckPt.position, _wallCheckSz);
        Gizmos.DrawWireSphere(_attackPt.transform.position, _attackSz);
        Gizmos.DrawWireCube(_LeftwallCheckPt.position,_wallCheckSz);

    }


    private void JumpAnim()
    {
        if (isJumping == true)
        {
            anim.SetBool("isJump", true);
        }
        else if ( isJumping == false && IsGrounded() == true)
        {
            anim.SetBool("isJump", false);

        }
    }
    void Flip()
    {
        
        if(playerMovement.x < 0 && isDead == false)
        {
            _sprite.flipX = true;
           
            
        } 
        else if (playerMovement.x > 0 && isDead == false)
        {
            _sprite.flipX = false;
        }
        
    }


    private void OnFire(InputValue value)
    {
        if (value.isPressed && isDead == false)
        {
            anim.SetBool("isAttacking", true);
        }
    }

    public void EndAttack()
    {
        anim.SetBool("isAttacking", false);
    }
  

    public void Attack()
    {
        CreateDust();
        Collider2D[] enemy = Physics2D.OverlapCircleAll(_attackPt.transform.position, _attackSz, enemies);
        foreach (Collider2D enemies in enemy)
        {
            print("Hit");
            
        }

    }

    void CreateDust()
    {
        dust.Play();
    }
    public void CreateExpo()
    {
        expo.Play();
    }

    public void Coin()
    {
        canDash = true;
        expoC.Play();
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        myRigidBody.gravityScale = 0f;
        myRigidBody.velocity = new Vector2(playerMovement.x * DashingPower, playerMovement.y * DashingPower);
        tr.emitting = true;
        if (isDashing && playerMovement.x != 0 && isDead == false)
        {
            animator.Play("Dash", -1, 0);
        } else if (isDashing && playerMovement.y != 0 && isDead == false)
        {
            isJumping = true;
        }
        yield return new WaitForSeconds(dashingTime);

        myRigidBody.gravityScale = originalGravity;
        isDashing = false;
        tr.emitting = false;
        isJumping = false;

        yield return new WaitForSeconds(DashingCD);
        canDash = true;
    }

    private IEnumerator Jump()
    {
        float force = jumpForce;


        if (IsGrounded() && isDead == false)
        {
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, force);
            CreateDust();
            isJumping = true;
            print("jump!");

        }
        else if (IsGrounded() == false && IsWallR() && isDead == false )
        {
            isJumping = true;            
            myRigidBody.velocity = new Vector2(-15f, force);
            animator.Play("Jump", -1, 0f);
            print("wallJUMP");
            tr.emitting = true;

        }
        else if (IsGrounded() == false && IsWallL() && isDead == false)
        {
            isJumping = true;
            myRigidBody.velocity = new Vector2(15f, force);
            animator.Play("Jump", -1, 0f);
            print("wallJUMP");
            tr.emitting = true;

        }

        yield return new WaitForSeconds(.1f);
        isJumping = false;
        tr.emitting = false;

    }

    
    public void Die()
    {
        StartCoroutine(FlashColor(.1f, Color.red));
    }
    
    private IEnumerator FlashColor(float duration, Color newColor)
    {
        animator.Play("Die");
        isDead = true;
        _sprite.flipX = false;
        expoD.Play();
        myRigidBody.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(1);
        myRigidBody.bodyType = RigidbodyType2D.Dynamic;
        CreateExpo();
        isDead = false;
        transform.position = startPosition;
        _sprite.color = newColor;
        yield return new WaitForSeconds(duration);
        _sprite.color = oldColor;
    }

    public void SpawnPT()
    {
        startPosition = transform.position;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        grounded = true;
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        grounded = false;
    //    }
    //}
}