using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GravityMovement : MonoBehaviour
{
    [SerializeField] private Transform player_transform;

    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;

    private bool isGrounded = false;
    public Transform isGroundedChecker1;
    public Transform isGroundedChecker2;
    public Transform isGroundedChecker3;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float rememberGroundedFor;
    private float lastTimeGrounded;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private const float FLOOR = -5.62f;
    private const float CEILING = 5.62f;
    private const float LEFT_WALL = -9.5f;
    private const float RIGHT_WALL = 9.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
        CheckNextLevel();
        CheckIfGrounded();
        CheckIfOutOfBounds();
    }

    private void CheckIfOutOfBounds()
    {
        if (transform.position.y > CEILING)
        {
            RestartQuit.Instance.RestartGame();
        }

        if (transform.position.y < FLOOR)
        {
            RestartQuit.Instance.RestartGame();
        }

        if (transform.position.x < LEFT_WALL)
        {
            RestartQuit.Instance.RestartGame();
        }

        if (transform.position.x > RIGHT_WALL)
        {
            RestartQuit.Instance.RestartGame();
        }
    }

    private void CheckNextLevel()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

            //Game over
            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(nextIndex);
            }
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        float horiz_velocity = x * speed;
        float vert_velocity = y * speed;

        if (!IsInputFieldSelected())
        {
            if (this.transform.eulerAngles.z == 0 || this.transform.eulerAngles.z == 180)
            {
                rb.velocity = new Vector2(horiz_velocity, rb.velocity.y);
            } else if (this.transform.eulerAngles.z == 90 || this.transform.eulerAngles.z == 270) {
                rb.velocity = new Vector2(rb.velocity.x, vert_velocity);
            }
        }

        if (x > 0)
        {
            player_transform.localScale = Vector3.one * 1f;
        }
        else if (x < 0)
        {
            player_transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    void Jump()
    {
        if (!IsInputFieldSelected() && Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor))
        {
            if (this.transform.eulerAngles.z == 0) 
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); //gravity down
            } else if (this.transform.eulerAngles.z == 180) {
                rb.velocity = new Vector2(rb.velocity.x, -jumpForce); //gravity up
            } else if (this.transform.eulerAngles.z == 90) {
                rb.velocity = new Vector2(-jumpForce, rb.velocity.y); //gravity right
            } else if (this.transform.eulerAngles.z == 270) {
                rb.velocity = new Vector2(jumpForce, rb.velocity.y); //gravity left
            }
        }
    }

    private bool IsInputFieldSelected()
    {
        if (RestartQuit.Instance != null)
        {
            return RestartQuit.Instance.IsAnyInputFieldSelected;
        }

        return false;
    }

    void CheckIfGrounded()
    {
        Collider2D colliders1 = Physics2D.OverlapCircle(isGroundedChecker1.position, checkGroundRadius, groundLayer);
        Collider2D colliders2 = Physics2D.OverlapCircle(isGroundedChecker2.position, checkGroundRadius, groundLayer);
        Collider2D colliders3 = Physics2D.OverlapCircle(isGroundedChecker3.position, checkGroundRadius, groundLayer);

        if ((colliders1 != null) || (colliders2 != null) || (colliders3 != null))
        {
            isGrounded = true;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }
}
