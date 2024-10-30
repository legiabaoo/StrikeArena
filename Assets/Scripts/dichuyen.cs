using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
public class dichuyen : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float maxVelocityChange = 10f;
    public float sprintSpeed = 14f;
    [Space]
    public float  ariControl = 0.5f;
    [Space]
    public float jumpHeight = 30f;
    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;
    private bool grounded = false;

     private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");

        Vector3 velocity = rb.velocity;
        animator.SetFloat("VelocityX", velocity.x);
        animator.SetFloat("VelocityZ", velocity.z);
    }
    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }
    private float fallMultiplier = 2.5f; // T?c ?? r?i nhanh h?n khi nh?y

    private void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            }
            else if (input.magnitude > 0.5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.deltaTime, velocity1.y, velocity1.z * 0.2f * Time.deltaTime);
                rb.velocity = velocity1;
            }
        }
        else
        {
            if (rb.velocity.y < 0) // Khi nhân v?t ?ang r?i xu?ng
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }

            if (input.magnitude > 0.5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * ariControl : walkSpeed * ariControl), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.deltaTime, velocity1.y, velocity1.z * 0.2f * Time.deltaTime);
                rb.velocity = velocity1;
            }
        }
        grounded = false;
    }




    public void thoatgame()
    {

    }
    Vector3 CalculateMovement(float _speed) {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= _speed;

         Vector3 velocity = rb.velocity;
      
        if (input.magnitude > 0.5f){
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            return (velocityChange);
        }else
        {
            return new Vector3();
        }
    }
}
