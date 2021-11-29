using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private Vector2 v1 = new Vector2(1000, 1000);


    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        if((int)(target.position.x) == (int)(transform.position.x) && (int)(target.position.y) == (int)(transform.position.y))
        {
            //gameObject.SetActive(false);
            gameObject.transform.position = v1;
        }

            AttemptMove<Player>(xDir, yDir);
    }

    protected void attackInvoke()
    {
        gameObject.transform.position = v1;
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        hitPlayer.LoseFood(playerDamage);
        if (gameObject.name == "Enemy2(Clone)")
        {
            SoundManager.instance.RandomizeSfx(enemyAttack2, enemyAttack2);
            //gameObject.SetActive(false);
            Invoke("attackInvoke", 0.15f);
            
        }
        else
            SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack1);
    }


}
