﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatShadow : MonoBehaviour {
    public static CatShadow instance;
    private bool isPatroller = false;
    public bool destroyEndPath = false;
    public bool stopEndPath = false;

    public float speed;
    public Animator animator;
    private bool directionChanged = true;
    private int direction = 0;
    SpriteRenderer spriteRenderer;

    public Transform[] targets;
    private int destPoint = 0;

    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        if (isPatroller)
        {
            GotoNextPoint();
        }

    }

    void GotoNextPoint()
    {
        float step = speed * Time.deltaTime;

        if (targets.Length == 0)
            return;

        ChangeDirection();

        transform.position = Vector3.MoveTowards(transform.position, targets[destPoint].position, step);

        float dist = Vector3.Distance(targets[destPoint].position, transform.position);
        if (dist < 0.4f)
        {
            animator.SetTrigger("changeState");
            if (destPoint + 1 == targets.Length && destroyEndPath)
            {
                Destroy(gameObject);
            }
            else if (destPoint + 1 == targets.Length && stopEndPath)
            {
                Stop();
                // Mudar animacao, ficar parado
            }
            else
            {
                destPoint = (destPoint + 1) % targets.Length;
            }

        }

    }

    void ChangeDirection()
    {
        if (Mathf.Abs(targets[destPoint].position.y - transform.position.y) <
            Mathf.Abs(targets[destPoint].position.x - transform.position.x))
        {
            if (targets[destPoint].position.x > transform.position.x)
                animator.SetInteger("direction", 2);
            else
                animator.SetInteger("direction", 3);
        }
        else
        {
            if (targets[destPoint].position.y > transform.position.y)
                animator.SetInteger("direction", 4);
            else
                animator.SetInteger("direction", 5);
        }
    }

    public void ChangePosition(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void Patrol()
    {
        isPatroller = true;
    }

    public void Stop()
    {
        isPatroller = false;
    }

    public void DestroyCat()
    {
        Destroy(gameObject);
    }
}
