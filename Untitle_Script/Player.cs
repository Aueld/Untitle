using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MovingObject
{
    private static readonly WaitForSeconds wait = new WaitForSeconds(0.01f);

    // 좌우 상하 이동
    [SerializeField] private int horizontal = 0;
    [SerializeField] private int vertical = 0;
    
    // 플레이어 데미지
    [SerializeField] private int wallDamage = 1;
    
    // 에너지 회복량
    [SerializeField] private int pointPerEnergy = 5;
    [SerializeField] private int pointPerEnergies = 15;
    
    // 레벨 리셋 딜레이
    [SerializeField] private float restartLevelDelay = 1f;

    [SerializeField] private TextMeshProUGUI EnergyText;
    [SerializeField] private TextMeshProUGUI BoomCountText;

    // 사운드
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip wallSound;
    [SerializeField] private AudioClip energySound;
    [SerializeField] private AudioClip gameOverSound;

    // 폭탄
    [SerializeField] private GameObject boom;
    private List<GameObject> booms = new List<GameObject>();

    private GlitchEffect hitEff;

    private Animator animator;
    private Transform thisTransform;

    private int energy;
    private int totalScore;
    private int boomCount = 3;

    private bool wallCheck = false;

    private CameraShake cameraShake;

    protected override void Start()
    {
        hitEff = Camera.main.GetComponent<GlitchEffect>();
        animator = GetComponent<Animator>();

        energy = GameManager.instance.playerEnergyPoints;
        totalScore = GameManager.instance.score;

        thisTransform = GetComponent<Transform>();

        EnergyText.text = "Energy: " + energy;
        BoomCountText.text = "X " + GameManager.instance.boomCount;

        cameraShake = Camera.main.GetComponent<CameraShake>();

        booms.Clear();
        for(int i = 0; i < boomCount; i++)
        {
            booms.Add(Instantiate(boom));
            booms[i].SetActive(false);
        }

        base.Start();

        //StartCoroutine(PCController());
    }

    private void OnDisable()
    {
        GameManager.instance.playerEnergyPoints = energy;
        GameManager.instance.score = totalScore;
    }

    private void Update()
    {
        // 위치값 계속 초기화
        horizontal = 0;
        vertical = 0;

        if (transform.position.x == 11 && transform.position.y == 7)
            return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            // nameof 멤버 이름을 문자열
            Invoke(nameof(Restart), restartLevelDelay);
            enabled = false;
        }
        else if (collision.tag == "Energy")
        {
            SoundManager.instance.RandomizeSfx(0.75f, energySound, energySound);
            energy += pointPerEnergy;

            totalScore += 30;

            EnergyText.text = "+ " + pointPerEnergy;
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Energies")
        {
            SoundManager.instance.RandomizeSfx(0.75f, energySound, energySound);
            energy += pointPerEnergies;

            totalScore += 10;

            EnergyText.text = "+ " + pointPerEnergies;
            collision.gameObject.SetActive(false);
        }
        else if(collision.tag == "Boom")
        {
            GameManager.instance.boomCount++;
            BoomCountText.text = "X " + GameManager.instance.boomCount;

            SoundManager.instance.RandomizeSfx(0.75f, energySound, energySound);
            energy += 10;

            totalScore += 50;

            EnergyText.text = "+ " + 10;
            collision.gameObject.SetActive(false);
        }
    }

    public void ClickBoom()
    {
        if (GameManager.instance.boomCount > 0)
        {
            GameManager.instance.boomCount--;
            BoomCountText.text = "X "+ GameManager.instance.boomCount;

            foreach (var boomf in booms)
            {
                if (!boomf.activeSelf)
                {
                    boomf.transform.position = gameObject.transform.position;
                    boomf.SetActive(true);
                    break;
                }
            }
        }
    }

    // Event Trigger Pointer Down (BaseEventData) => Player
    public void ButtonDown(string nKey)
    {
        // 플레이어 턴인지 판단
        if (!GameManager.instance.playersTurn) return;
        
        // 플레이어가 포탈 위치인지 판단
        if (transform.position.x == 11 && transform.position.y == 7) return;

        bool check = true;

        switch (nKey)
        {
            case "Up":      // up
                check = CheckPosition(0, 10, 7, 7);     // 포탈 아래측에서 진입 가능
                vertical = 1;
                break;
            case "Down":    // Down
                check = CheckPosition(0, 11, 0, 0);
                vertical = -1;
                break;
            case "Right":   // Right
                thisTransform.localScale = new Vector3(1, 1, 1);
                check = CheckPosition(11, 11, 0, 6);    // 포탈 좌측에서 진입 가능
                horizontal = 1;
                break;
            case "Left":    // Left
                thisTransform.localScale = new Vector3(-1, 1, 1);
                check = CheckPosition(0, 0, 0, 7);
                horizontal = -1;
                break;
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f);
        Collider2D targetCol = null;

        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Enemy"))
                {
                    targetCol = cols[i];
                    break;
                }
                else
                    targetCol = null;
            }
        }

        if (targetCol != null)
            AttemptMove<Enemy>(horizontal, vertical);

        // 이동시
        if (check && (horizontal != 0 || vertical != 0))
            AttemptMove<Wall>(horizontal, vertical);
    }

    // 이동 시도
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        energy--;
        totalScore += 10;
        EnergyText.text = "Energy: " + energy;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(0.75f, moveSound, moveSound);
            wallCheck = false;
        }
        else if (!wallCheck)
        {
            SoundManager.instance.RandomizeSfx(0.75f, wallSound, wallSound);
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    // 이동 할 수 없을때
    protected override void OnCantMove<T>(T component)
    {
        cameraShake.ShakeForTimeAdd(0.3f);
        if (component as Wall)
        {
            wallCheck = true;

            Wall hitWall = component as Wall;

            totalScore += 25;

            if(hitWall.DamageWall(wallDamage))
                animator.SetTrigger("playerAttack");

            // nameof 멤버 이름을 문자열
            Invoke(nameof(WallCheck), 0.5f);
        }
        if (component as Enemy)
        {
            Enemy hitEnemy = component as Enemy;

            int e = hitEnemy.IsHit();

            energy += e;

            totalScore += 50;
            if (e == 0)
                animator.SetTrigger("playerAttack");
            else
                EnergyText.text = "+ " + pointPerEnergy;

        }
    }

    // 피격시
    public void LoseEnergy(int loss)
    {
        cameraShake.ShakeForTimeAdd(0.5f);

        StartCoroutine(HitEffact());
        animator.SetTrigger("playerHit");
        energy -= loss;
        EnergyText.text = "- " + loss + " Energy: " + energy;
        CheckIfGameOver();
    }

    private void WallCheck()
    {
        wallCheck = false;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ReSetting()
    {
        GameManager.instance.level = 0;
        GameManager.instance.score = 0;
        GameManager.instance.playerEnergyPoints = 100;
        
        Start();

        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        GameManager.instance.enabled = true;
    }

    // 게임 오버 판단
    private void CheckIfGameOver()
    {
        if (energy <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    // 위치 판단 후 true false
    private bool CheckPosition(int x1, int x2, int y1, int y2)
    {
        if (transform.position.x >= x1 && transform.position.x <= x2
            && transform.position.y >= y1 && transform.position.y <= y2)
            return false;
        
        return true;
    }

    // 피격 이펙트
    private IEnumerator HitEffact()
    {
        float time = 20f;
        
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

    #region PC Controller
    // ---------------------------------------------------------------------------------------

//    private static WaitForSeconds movWait = new WaitForSeconds(0.14f);

//    private IEnumerator PCController()
//    {
//        while (true)
//        {

//            if (!GameManager.instance.playersTurn)
//                yield return 0;

//#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

//            horizontal = (int)Input.GetAxisRaw("Horizontal");
//            vertical = (int)Input.GetAxisRaw("Vertical");

//            if (horizontal != 0)
//                vertical = 0;

//            // 좌우 반전 기능
//            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
//                thisTransform.localScale = new Vector3(-1, 1, 1);
//            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
//                thisTransform.localScale = new Vector3(1, 1, 1);
//#else

//                if (Input.touchCount > 0)
//                {
//                    Touch touch = Input.touches[0];

//                    if (touch.phase == TouchPhase.Began)
//                    {
//                        touchOrigin = touch.position;
//                    }
//                    else if(touch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
//                    {
//                        Vector2 touchEnd = touch.position;
//                        float x = touchEnd.x - touchOrigin.x;
//                        float y = touchEnd.y - touchOrigin.y;
//                        touchOrigin.x = -1;
//                        if (Mathf.Abs(x) > Mathf.Abs(y)){
//                            horizontal = x > 0 ? 1 : -1;

//                            if(horizontal == -1) transformThis.localScale = new Vector3 (-1, 1, 1);
//                            if(horizontal == 1) transformThis.localScale = new Vector3 (1, 1, 1);
//                        }
//                        else
//                            vertical = y > 0 ? 1 : -1;
//                    }
//                }
//#endif

//            if (horizontal != 0 || vertical != 0)
//            {
//                if (vertical > 0)
//                    CheckPosition(0, 10, 7, 7);
//                else if (vertical < 0)
//                    CheckPosition(0, 11, 0, 0);
//                if (horizontal > 0)
//                    CheckPosition(11, 11, 0, 6);
//                else if (horizontal < 0)
//                    CheckPosition(0, 0, 0, 7);

//                AttemptMove<Wall>(horizontal, vertical);
//            }

//            yield return movWait;
//        }
//    }

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

    #endregion
}
