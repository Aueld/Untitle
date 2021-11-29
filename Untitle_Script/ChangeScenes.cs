using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    
    public void OnClickButton()
    {
        player.ReSetting();
        //Debug.Log("재시작 가동");
    }
}