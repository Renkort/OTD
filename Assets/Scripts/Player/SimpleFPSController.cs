using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.FPS
{
    
    public class SimpleFPSController : MonoBehaviour
    {
        [SerializeField] private GameObject cam;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float speed = 12f;
        [SerializeField] private float jumpHeight = 4f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private bool isGrounded;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private Transform groundCheck;

        [SerializeField] private float sensitivity = 300f;
        [SerializeField] private float topClamp = -90f, bottomClamp = 90f;
        private Vector3 moveInput;
        private Vector3 mouseInput;
        private float xRotation = 0f, yRotation = 0f;
        Vector3 velocity;
        public bool CanMove = true;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            controller = GetComponent<CharacterController>();
        }
        private void Update()
        {
            HandleMovement();
            HandleCameraRotation();
        }

        private void HandleMovement()
        {
            if (!CanMove)
                return;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0f)
                velocity.y = -2f;

            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.z;
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        private void HandleCameraRotation()
        {
            mouseInput.x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            mouseInput.y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            xRotation -= mouseInput.y;
            xRotation = Math.Clamp(xRotation, topClamp, bottomClamp);
            yRotation += mouseInput.x;
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
            //transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}
