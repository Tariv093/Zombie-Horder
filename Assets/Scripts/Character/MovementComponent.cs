using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    
    public class MovementComponent : MonoBehaviour
    {
        [SerializeField] private float WalkSpeed;
        [SerializeField] private float RunSpeed;
        [SerializeField] private float JumpForce;
        
        private Vector2 InputVector = Vector2.zero;
        private Vector3 MoveDirection = Vector3.zero;
        
        //Comp
        private Animator PlayerAnimator;
        private PlayerController PlayerController;
        private Rigidbody PlayerRigidbody;
        
        
        //Reference 
        private Transform PlayerTransform;

        
        //Animator Hashes
        private readonly int MovementXHash = Animator.StringToHash("MovementX");
        private readonly int MovementZHash = Animator.StringToHash("MovementZ");
        private readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private readonly int IsJumpingHash = Animator.StringToHash("IsJumping");

        private void Awake()
        {
            PlayerController = GetComponent<PlayerController>();
            PlayerAnimator = GetComponent<Animator>();
            PlayerRigidbody = GetComponent<Rigidbody>();

            PlayerTransform = transform;
        }

        private void Update()
        {
            if (PlayerController.IsJumping) return;
            
            if (!(InputVector.magnitude > 0))  MoveDirection = Vector3.zero;
            
            MoveDirection = PlayerTransform.forward * InputVector.y + PlayerTransform.right * InputVector.x;

            float currentSpeed = PlayerController.IsRunning ? RunSpeed : WalkSpeed;

            Vector3 movementDirection = MoveDirection * (currentSpeed * Time.deltaTime);
            
            PlayerTransform.position += movementDirection;
        }

        public void OnMovement(InputValue value)
        {
            InputVector = value.Get<Vector2>();
            
            PlayerAnimator.SetFloat(MovementXHash, InputVector.x);
            PlayerAnimator.SetFloat(MovementZHash, InputVector.y);
        }

        public void OnRun(InputValue button)
        {
            PlayerController.IsRunning = button.isPressed;
            PlayerAnimator.SetBool(IsRunningHash, button.isPressed);
        }

        public void OnJump(InputValue button)
        {
            PlayerController.IsJumping = true;
            PlayerAnimator.SetBool(IsJumpingHash, true);
            
            PlayerRigidbody.AddForce((transform.up + MoveDirection) * JumpForce, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Ground") && !PlayerController.IsJumping) return;

            PlayerController.IsJumping = false;
            PlayerAnimator.SetBool(IsJumpingHash, false);
        }
    }
}
