using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;

public class PlayerController : NetworkBehaviour
{
    private Vector3 direction;
    [Header("Movement")]
    public float speed;
    public float jumpSpeed;
    public float rotationSpeed;

    [Header("References")]
    public Transform orientation;
    public LayerMask groundLayerMask;
    private float playerHeight;
    private Animator playerAnimator;

    [Header("Sounds")]
    public AudioSource source;
    public AudioClip jumpClip;
    public AudioClip hitClip;


    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    public Transform cameraTransform;
    private bool isGrounded;


    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        playerHeight = 3; //Estimated
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsOwner) return;

        //Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);
        //if (isGrounded) rb.drag = 5;
       // else rb.drag = 0;

        //Orientation
        Vector3 viewDir = transform.position - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;


       // horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");
        //direction = orientation.forward * verticalInput + orientation.right * horizontalInput;

        Jump();
        Move();
        Attack();

        if (direction != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, direction.normalized, Time.deltaTime * rotationSpeed);
    }

    void FixedUpdate()
    {
        //Gravity Boost
        rb.AddForce(Vector3.down * 10);
    }

    public override void OnNetworkSpawn()
    {

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Debug.Log("Jump");
            source.PlayOneShot(jumpClip);
            //direction = orientation.forward * verticalInput + orientation.right * horizontalInpuoh set;
             rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            //rb.velocity += new Vector3(0, jumpSpeed, 0);
        }

    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerAnimator.SetTrigger("attackTrigger");
        }
    }
    void Move() {
        direction = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey("w"))
        {
            direction += cameraTransform.forward;
            direction.y = 0;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey("s"))
        {
            direction += cameraTransform.forward * -1;
            direction.y = 0;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey("a"))
        {
            direction += cameraTransform.right * -1;
            direction.y = 0;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey("d"))
        {
            direction += cameraTransform.right;
            direction.y = 0;
        }

        if (direction != Vector3.zero) playerAnimator.SetBool("isMoving", true);
        else playerAnimator.SetBool("isMoving", false);
        //   direction = Vector3.Normalize(new Vector3(direction.x, 0, direction.z));

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    public bool GetIsMoving()
    {
        if (direction != Vector3.zero) return true;
        return false;
    }

    public bool GetGrounded()
    {
        return isGrounded;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<EnemyController>())
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

            Vector3 directionOfStrike = transform.position - enemy.transform.position;
            rb.AddForce(directionOfStrike * 1000);
            source.PlayOneShot(hitClip);
        }
    }
}
