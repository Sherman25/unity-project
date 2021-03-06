﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    //public Transform enemy;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator animator;
    public Collider2D collider;

    public float speed = 200f;
    public int attackDamage = 40;
    public int maxHealth = 100;
    private int currentHealth;
    public float nextWaypointDistance = 3f;
    public float attackRange = 0.5f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    bool canMove = true;

    Seeker seeker;
    Rigidbody2D rb;

    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone()) 
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (canMove)
        {
            rb.AddForce(force);
            animator.SetBool("Walk", true);
        }
        else
        {
            rb.AddForce(new Vector2(0, 0));
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        //animator.SetFloat("Force", Mathf.Abs(force.x));
        
        if (force.x > 0)
        {
            transform.localScale = new Vector3(5f, 5f, 1f);
        }
        else if (force.x < 0)
        {
            transform.localScale = new Vector3(-5f, 5f, 1f);
        }
        

        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayer);
        if (hitEnemy != null && canMove == true)
        {
            canMove = false;
            animator.SetBool("Walk", false);
            Attack(hitEnemy);
        }
    }

    void Remove()
    {
        collider.gameObject.SetActive(false);
    }

    void Attack(Collider2D hitEnemy)
    {
        // Play an atack animation
        //rb.AddForce(new Vector2(0, 0));
        animator.SetFloat("Force", 0);
        animator.SetTrigger("Attack");
        animator.SetBool("Attacking", true);
    }

    public void CheckFireArea()
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayer);
        if (hitEnemy != null)
        {
            hitEnemy.GetComponent<PlayerController>().TakeDamage(attackDamage);
            // Damage
            Debug.Log("We hit " + hitEnemy.name);
        }
    }

    public void TakeDamage(int damage)
    {
        //rb.AddForce(new Vector2(0, 0));
        canMove = false;
        animator.SetBool("Walk", false);
        currentHealth -= damage;

        // Play hearth animation
        animator.SetTrigger("Hurt");

        // Play fail animation
        if (currentHealth <= 0)
        {
            Fail();
        }
    }

    void Fail()
    {
        canMove = false;
        Debug.Log("Enemy fail!");
        animator.SetTrigger("Fail");
        animator.SetBool("Failed", true);
        animator.SetBool("Walk", false);
    }

    void SetFailed()

    {
        animator.SetBool("Failed", true);

    }

    public void SetCanMove()
    {
        canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
