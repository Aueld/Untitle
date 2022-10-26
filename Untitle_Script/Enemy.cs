using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public bool doubleMove;

    public int HP;
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private Vector2 v1 = new Vector2(1000, 1000);
    private CameraShake cameraShake;
    
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
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

        // 잡아먹기
        if ((int)(target.position.x) == (int)(transform.position.x)
            && (int)(target.position.y) == (int)(transform.position.y))
        {
            gameObject.transform.position = v1;
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f);
        Collider2D targetCol = null;

        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player"))
                {
                    targetCol = cols[i];
                    break;
                }
                else
                    targetCol = null;
            }
        }

        if (targetCol != null)
            AttemptMove<Player>(xDir, yDir);
        else
            AttemptMove<Wall>(xDir, yDir);
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public int IsHit()
    {
        HP--;

        if (HP < 0)
        {
            cameraShake.ShakeForTimeAdd(1.2f);
            AttackInvoke();
            return 10;
        }

        return 0;
    }

    public void IsBoom()
    {
        gameObject.transform.position = v1;
    }

    protected void AttackInvoke()
    {
        gameObject.transform.position = v1;
    }

    protected override void OnCantMove<T>(T component)
    {
        if (component as Player)
        {
            Player hitPlayer = component as Player;

            animator.SetTrigger("enemyAttack");

            hitPlayer.LoseEnergy(playerDamage);

            if (gameObject.name == "Enemy2(Clone)")
            {
                SoundManager.instance.RandomizeSfx(0.75f, enemyAttack2, enemyAttack2);
                Invoke(nameof(AttackInvoke), 0.15f);
            }
            else
                SoundManager.instance.RandomizeSfx(0.75f, enemyAttack1, enemyAttack1);
        }
        if (component as Wall)
        {
            Wall wall = component as Wall;

            animator.SetTrigger("enemyAttack");
            SoundManager.instance.RandomizeSfx(0.75f, enemyAttack1, enemyAttack1);

            wall.DamageWall(playerDamage / 5);
        }
    }
}
