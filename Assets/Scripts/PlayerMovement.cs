﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController controller;
    public Animator animator;
    public float runSpeed;

    private float horizontalMove;
    private float verticalMove;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal") * runSpeed;
        verticalMove = Input.GetAxis("Vertical") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove)) ;
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove, verticalMove);
    }
}
