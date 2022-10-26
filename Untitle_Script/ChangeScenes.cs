using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public Player player;

    private string log;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    
    public void OnClickButton()
    {
        GPGSBinder.Inst.Login((success, localUser) =>
        log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}");

        GPGSBinder.Inst.ReportLeaderboard(GPGSIds.leaderboard_best_score, GameManager.instance.score, success => log = $"{success}");
        
        GPGSBinder.Inst.ShowTargetLeaderboardUI(GPGSIds.leaderboard_best_score);


        player.ReSetting();

        //Debug.Log("재시작 가동");
    }
}