using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleExit : MonoBehaviour
{
    private void Start()
    {
        string log;

        GPGSBinder.Inst.Login((success, localUser) =>
        log = $"{success}, {localUser.userName}, {localUser.id}, {localUser.state}, {localUser.underage}");
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
