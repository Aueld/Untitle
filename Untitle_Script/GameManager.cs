using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static WaitForSeconds waitSec = new WaitForSeconds(0.1f);

    public float levelStartDelay = 2f;
    public static GameManager instance = null;
    
    public BoardManager boardScript;

    
    public int playerEnergyPoints = 100;
    public int score = 0;
    public int level = 1;
    public int boomCount = 3;

    [HideInInspector] public bool playersTurn = true;

    private TextMeshProUGUI levelText;
    private GameObject levelImage;
    private GameObject continueImage;
    private bool lastLevel = false;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    private Vector3 nowEnemyPos;

    void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            if (lastLevel)
            {
                level++;
                InitGame();
            }
            else
                lastLevel = true;
        }
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();

        if (continueImage == null)
        {
            continueImage = GameObject.Find("ConImage");
        }

        levelText.text = "B " + level+"F";

        continueImage.SetActive(false);
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        score *= (int)Mathf.Log(level, 2f);
        levelText.text = "Entered the " + level + "F\nTotalScore : " + score;

        // 스코어 저장
        //

        levelImage.SetActive(true);
        continueImage.SetActive(true);
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        
        yield return waitSec;

        if (enemies.Count == 0)
            yield return waitSec;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].doubleMove)
                enemies[i].MoveEnemy();

            enemies[i].MoveEnemy();

            yield return null;
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}