using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    private string log;

    public void OnClickStart()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnGPGSLogIn()
    {
        GPGSBinder.Inst.Login((success, localUser) =>
        log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}");

        GPGSBinder.Inst.ShowTargetLeaderboardUI(GPGSIds.leaderboard_best_score);
    }
}
