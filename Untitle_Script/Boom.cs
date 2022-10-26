using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    [SerializeField] private AudioClip boomSound;

    private Animator anim;
    private Player player;
    private List<Enemy> enemies = new List<Enemy>();
    private List<Wall> walls = new List<Wall>();

    private Collider2D[] cols;

    private bool boomCheck;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        boomCheck = false;

        cols = null;

        enemies.Clear();
        walls.Clear();
    }

    private void Update()
    {
        cols = Physics2D.OverlapCircleAll(transform.position, 1f);

        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player"))
                {
                    player = cols[i].GetComponent<Player>();
                }
                else if (cols[i].CompareTag("Enemy"))
                {
                    enemies.Add(cols[i].GetComponent<Enemy>());
                }
                else if (cols[i].CompareTag("Wall"))
                {
                    walls.Add(cols[i].GetComponent<Wall>());
                }
            }
        }

        if (!boomCheck && Vector2.Distance(player.gameObject.transform.position, transform.position) > 1.5f)
        {
            boomCheck = true;
            StartCoroutine(BoomFire());
            SoundManager.instance.RandomizeSfx(0.15f, boomSound, boomSound);
            Camera.main.GetComponent<CameraShake>().ShakeForTimeAdd(1.6f);
            return;
        }
    }

    private IEnumerator BoomFire()
    {
        foreach (var enemy in enemies)
        {
            enemy.IsBoom();
        }
        foreach (var wall in walls)
        {
            wall.BoomWall();
        }
        anim.SetBool("Fire", true);

        yield return new WaitForSeconds(0.8f);

        gameObject.SetActive(false);
    }
}
