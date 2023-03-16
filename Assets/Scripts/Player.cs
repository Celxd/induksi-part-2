using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 50f;
    [SerializeField] float sensitivity = .05f;

    public Vector2 turn;
    private Rigidbody rb;
    private float currentSpeed;
    public int jumpsLeft = 2;
    private bool isSprinting;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        isSprinting = true;
        animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        currentSpeed = isSprinting ? speed * 2 : speed;
        currentSpeed *= 2;
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * currentSpeed;
        
        if (movement.magnitude > 0 && rb.velocity.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.TransformDirection(movement));
            Quaternion clampedRotation = Quaternion.Euler(
                Mathf.Clamp(targetRotation.eulerAngles.x, -90f, 90f),
                targetRotation.eulerAngles.y,
                targetRotation.eulerAngles.z
            );
            transform.rotation = Quaternion.Slerp(transform.rotation, clampedRotation, sensitivity);
        }

        if(Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
        {
            if (jumpsLeft == 2)
            {
                animator.ResetTrigger("DoubleJump");
                animator.SetTrigger("Jump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
            }
            else if (jumpsLeft == 1)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("DoubleJump");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
            }
            
            jumpsLeft--;
        }

        float posY = movement.z;
        float posX = movement.x;

        animator.SetFloat("PosX", posX);
        animator.SetFloat("PosY", posY);
    }
    void FixedUpdate()
    {
        rb.AddForce(transform.TransformDirection(Physics.gravity) * Time.fixedDeltaTime * 800);
    }

    private void OnCollisionEnter(Collision collision)
    {
        jumpsLeft = 2;
    }
}
