using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    private static WaitForSeconds wait = new WaitForSeconds(0.01f);

    public int horizontal = 0;
    public int vertical = 0;
    public int wallDamage = 1;
    public int pointPerEnergy = 5;
    public int pointPerEnergies = 15;
    public float restartLevelDelay = 1f;
    public Text EnergyText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip energySound1;
    public AudioClip energySound2;
    public AudioClip gameOverSound;

    private GlitchEffect hitEff;

    private Animator animator;
    private Transform transformThis;
    private Vector2 touchOrigin = -Vector2.one;
    private int food;
    private int totalScore;
    private bool wallCheck = false;

    protected override void Start()
    {
        hitEff = Camera.main.GetComponent<GlitchEffect>();
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerEnergyPoints;
        totalScore = GameManager.instance.score;

        transformThis = GetComponent<Transform>();

        EnergyText.text = "Energy: " + food;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerEnergyPoints = food;
        GameManager.instance.score = totalScore;
    }

    
    public void ButtonDown(string nkey)
    {
        if (!GameManager.instance.playersTurn) return;
        if (transform.position.x == 11 && transform.position.y == 7) return;
                

        switch (nkey)
        {
            case "Up": // up
                CheckPosition(0, 10, 7, 7);
                vertical = 1;
                break;
            case "Down": // Down
                CheckPosition(0, 11, 0, 0);
                vertical = -1;
                break;
            case "Right": // Right
                transformThis.localScale = new Vector3(1, 1, 1);
                CheckPosition(11, 11, 0, 6);
                horizontal = 1;
                break;
            case "Left": // Left
                transformThis.localScale = new Vector3(-1, 1, 1);
                CheckPosition(0, 0, 0, 7);
                horizontal = -1;
                break;
        }
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }
    
    void Update()
    {
        horizontal = 0;
        vertical = 0;
    }

    //update 더미
    /*

                if (!GameManager.instance.playersTurn) return;




        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

                horizontal = (int)Input.GetAxisRaw("Horizontal");
                vertical = (int)Input.GetAxisRaw("Vertical");

                if (horizontal != 0) 
                    vertical = 0;

                // 좌우 반전 기능
                if (Input.GetKey(KeyCode.LeftArrow))
                    transformThis.localScale = new Vector3(-1, 1, 1);
                else if (Input.GetKey(KeyCode.RightArrow))
                    transformThis.localScale = new Vector3(1, 1, 1);
        #else

                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchOrigin = touch.position;
                    }
                    else if(touch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
                    {
                        Vector2 touchEnd = touch.position;
                        float x = touchEnd.x - touchOrigin.x;
                        float y = touchEnd.y - touchOrigin.y;
                        touchOrigin.x = -1;
                        if (Mathf.Abs(x) > Mathf.Abs(y)){
                            horizontal = x > 0 ? 1 : -1;

                            if(horizontal == -1) transformThis.localScale = new Vector3 (-1, 1, 1);
                            if(horizontal == 1) transformThis.localScale = new Vector3 (1, 1, 1);
                        }
                        else
                            vertical = y > 0 ? 1 : -1;
                    }
                }
        #endif

                if (horizontal != 0 || vertical != 0)
                    AttemptMove<Wall>(horizontal, vertical);

        */

    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        food--;
        totalScore += 10;
        EnergyText.text = "Energy: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound1);
            wallCheck = false;
        }
        else if (!wallCheck)
        {
            SoundManager.instance.RandomizeSfx(moveSound2, moveSound2);
        }
        //else if (!Move(xDir, yDir, out hit))
        //{
        //    food++;
        //    totalScore -= 10;
        //}

        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (collision.tag == "Food")
        {
            SoundManager.instance.RandomizeSfx(energySound1, energySound1);
            food += pointPerEnergy;
            totalScore += 30;
            EnergyText.text = "+ " + pointPerEnergy;
            collision.gameObject.SetActive(false);
        }
        else if(collision.tag == "Soda")
        {
            SoundManager.instance.RandomizeSfx(energySound2, energySound2);
            food += pointPerEnergies;
            totalScore += 10;
            EnergyText.text = "+ " + pointPerEnergies;
            collision.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        //food--;
        wallCheck = true;
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        totalScore += 25;
        animator.SetTrigger("playerAttack");
        Invoke("WallCheck", 0.5f);
    }

    private void WallCheck()
    {
        wallCheck = false;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void LoseFood(int loss)
    {
        StartCoroutine(HitEffact());
        animator.SetTrigger("playerHit");
        food -= loss;
        EnergyText.text = "- " + loss + " Energy: " + food;
        CheckIfGameOver();
    }

    public void ReSetting()
    {
        GameManager.instance.level = 0;
        GameManager.instance.playerEnergyPoints = 100;
        GameManager.instance.score = 0;
        Start();
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        GameManager.instance.enabled = true;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    private void CheckPosition(int x1, int x2, int y1, int y2)
    {
        if (transform.position.x >= x1 && transform.position.x <= x2 && transform.position.y >= y1 && transform.position.y <= y2)
        {
            food++;
            totalScore -= 10;
        }

    }

    private IEnumerator HitEffact()
    {
        float time = 40f;
        
        hitEff.colorIntensity = 1f;
        hitEff.flipIntensity = 1f;

        while (time > 0f)
        {
            time--;
            if(time < 2f)
            {
                hitEff.colorIntensity = 0f;
                hitEff.flipIntensity = 0f;
                break;
            }
            yield return wait;
        }
    }
}
