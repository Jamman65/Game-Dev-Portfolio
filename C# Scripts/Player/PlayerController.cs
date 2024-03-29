﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JO
{
    public class PlayerController : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        public  Vector3 moveDirection;
        public float jumpForce = 10;
        public PlayerManager playerManager;
        public CapsuleCollider characterCollider;
        public CapsuleCollider characterColliderBlocker;
        PlayerStats playerstats;

        CameraHandler cameraHandler;

        [HideInInspector]
        public Transform myTransform;
     
        public PlayerAnimatorManager animatorHandler;
        public CharacterStats characterstats;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f; // Start of ray cast to ground
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f; // Distance before falling animation
        [SerializeField]
        float groundDirectionRayDistance = 0.2f; // Distance to ground ray cast
        public LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Stamina Costs")]
        [SerializeField]
        float rollStaminaCost = 15;
        [SerializeField]
        float backstepStaminaCost = 10;
        [SerializeField]
        float sprintStaminaCost = 1;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallingSpeed = 45;



        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerstats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (characterstats.isDead)
            {
                return;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<PlayerAnimatorManager>();
            playerManager = GetComponent<PlayerManager>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initalize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
            Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
        }


   

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        public void HandleRotation(float delta)
        {
            if (animatorHandler.canRotate)
            {
                if (playerManager.isAimingArrow)
                {
                    Quaternion targetRotation = Quaternion.Euler(0, cameraHandler.cameraTransform.eulerAngles.y, 0);
                    Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = playerRotation;
                }
                else {
                    if (inputHandler.lockOnFlag)
                    {
                        if (inputHandler.sprintFlag || inputHandler.rollFlag)
                        {
                            Vector3 targetDirection = Vector3.zero;
                            targetDirection = cameraHandler.cameraTransform.forward * inputHandler.Vertical;
                            targetDirection += cameraHandler.cameraTransform.right * inputHandler.Horizontal;
                            targetDirection.Normalize();
                            targetDirection.y = 0;

                            if (targetDirection == Vector3.zero)
                            {
                                targetDirection = transform.forward;
                            }

                            Quaternion TargetDir = Quaternion.LookRotation(targetDirection);
                            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, TargetDir, rotationSpeed * Time.deltaTime);

                            transform.rotation = targetRotation;
                        }

                        else
                        {
                            //rotates the players camera to the locked on object
                            Vector3 rotationDirection = moveDirection;
                            rotationDirection = cameraHandler.currentLockOnTarget.position - transform.position;
                            rotationDirection.y = 0;
                            rotationDirection.Normalize();
                            Quaternion tr = Quaternion.LookRotation(rotationDirection);
                            Quaternion targetrotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                            transform.rotation = targetrotation;
                        }


                    }
                    else
                    {
                        Vector3 targetDir = Vector3.zero;
                        float moveOverride = inputHandler.moveAmount;

                        targetDir = cameraObject.forward * inputHandler.Vertical;
                        targetDir += cameraObject.right * inputHandler.Horizontal;

                        targetDir.Normalize();
                        targetDir.y = 0;


                        if (targetDir == Vector3.zero)
                            targetDir = myTransform.forward;

                        float rs = rotationSpeed;

                        Quaternion tr = Quaternion.LookRotation(targetDir);
                        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

                        myTransform.rotation = targetRotation;
                    }
                }
            }

                

            
        }

        #endregion

        public void HandleMovement(float delta)
        {

            if (inputHandler.rollFlag)
            {
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
                return;
            }

            if (playerManager.isInteracting)
            {
                return;
            }

            if(playerstats.staminaLevel <= 0)
            {
                return;
            }
                
        

            moveDirection = cameraObject.forward * inputHandler.Vertical;
            moveDirection += cameraObject.right * inputHandler.Horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;



            float speed = movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
                playerstats.TakeStamina(sprintStaminaCost);
            }

            //if(inputHandler.moveAmount < 0.5)
            //{
            //    moveDirection *= movementSpeed;
            //    playerManager.isSprinting = false;
            //}

            else
            {
                moveDirection *= speed;
                playerManager.isSprinting = false;
            }
            

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.Vertical, inputHandler.Horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }

            

            
        }

        public void handleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
            {
                return;
            }

            if(playerstats.currentStamina <= 0)
            {
                return;
            }

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.Vertical;
                moveDirection += cameraObject.right * inputHandler.Horizontal;

                if(inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    playerstats.TakeStamina(rollStaminaCost);
                }

                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                    playerstats.TakeStamina(backstepStaminaCost);
                }
            }
        }


        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                
                rigidbody.AddForce(moveDirection * fallingSpeed / 6f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
            if(Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    if(inAirTimer > 0.5f)
                    {
                        Debug.Log("you were in the air for" + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Land", true);
                        inAirTimer = 0;
                    }

                    else
                    {
                        animatorHandler.PlayTargetAnimation("Locomotion", false);
                        animatorHandler.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }

                
            }

            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (playerManager.isInAir == false)
                {
                    if(playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            if(playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
            
            }

            else
            {
                myTransform.position = targetPosition;
            }

            if (playerManager.isGrounded)
            {
                if(playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
                }

                else
                {
                    myTransform.position = targetPosition;
                }
            }
        }

        public void HandleJumping()
        {
            if (playerManager.isInteracting)
            {
                return;
            }
            if (playerstats.currentStamina <= 0)
            {
                return;
            }

            if (inputHandler.Jump_input)
            {
                if(inputHandler.moveAmount > 0)
                {
                    moveDirection = cameraObject.forward * inputHandler.Vertical;
                    moveDirection += cameraObject.right * inputHandler.Horizontal;

                    rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                    animatorHandler.PlayTargetAnimation("Jump", true);
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }
    }

}