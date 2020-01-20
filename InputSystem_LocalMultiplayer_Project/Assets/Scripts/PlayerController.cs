﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Camera
    private Camera mainCamera;

    [Header("Physics")]
    public Rigidbody playerRigidbody;

    [Header("Animation")]
    public Animator playerAnimator;

    [Header("Input")]
    public PlayerInput playerInput;
    private string actionMapGameplay = "Player Controls";
    private string actionMapMenu = "Menu";
    
    private Vector3 inputDirection;
    private Vector2 movementInput;
    private bool currentInput = false;

    [Header("Movement Settings")]
    public float movementSpeed = 3;
    public float smoothingSpeed = 1;
    private Vector3 currentDirection;
    private Vector3 rawDirection;
    private Vector3 smoothDirection;
    private Vector3 movement;

    void Start()
    {
        FindCamera();
    }

    void FindCamera()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        CalculateMovementInput();
        CalculateDesiredDirection();
        ConvertDirectionFromRawToSmooth();
        MoveThePlayer();
        AnimatePlayerMovement();
        TurnThePlayer();
    }

    void CalculateMovementInput()
    {

        if(inputDirection == Vector3.zero)
        {
            currentInput = false;
        }
        else if(inputDirection != Vector3.zero)
        {
            currentInput = true;
        }
    }

    void CalculateDesiredDirection()
    {
        //Camera Direction
		var cameraForward = mainCamera.transform.forward;
		var cameraRight = mainCamera.transform.right;

		cameraForward.y = 0f;
		cameraRight.y = 0f;

        rawDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
    }

    void ConvertDirectionFromRawToSmooth()
    {   
        if(currentInput == true)
        {
            smoothDirection = Vector3.Lerp(smoothDirection, rawDirection, Time.deltaTime * smoothingSpeed);
        } else if(currentInput == false)
        {
            smoothDirection = Vector3.zero;
        }
        
    }

    void MoveThePlayer()
    {
        if(currentInput == true)
        {
            movement.Set(smoothDirection.x, 0f, smoothDirection.z);
            movement = movement.normalized * movementSpeed * Time.deltaTime;
            playerRigidbody.MovePosition(transform.position + movement);
        }

    }

    void TurnThePlayer()
    {
        if(currentInput == true)
        {
            Quaternion newRotation = Quaternion.LookRotation(smoothDirection);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void AnimatePlayerMovement()
    {
        playerAnimator.SetFloat("Movement", inputDirection.sqrMagnitude);
    }

    //Callback from the new Input System
    private void OnMovement(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();
        inputDirection = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    private void OnAttack(InputValue value)
    {
        if(value.isPressed)
        {
            playerAnimator.SetTrigger("Attack");
        }
    }

    //Callback from the new Input System
    private void OnOpenPauseMenu(InputValue value)
    {

        if(value.isPressed)
        {
            GameManager.Instance.TogglePauseMenu(true);

            //Switch this Player Input's Action Map
            playerInput.SwitchCurrentActionMap(actionMapMenu);
        }
    }

    private void OnClosePauseMenu(InputValue value)
    {
        if(value.isPressed)
        {
            GameManager.Instance.TogglePauseMenu(false);

            //Switch this PlayerInput's Action Map
            playerInput.SwitchCurrentActionMap(actionMapGameplay);
        }
    }


}
