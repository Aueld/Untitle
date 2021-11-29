using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleExit : MonoBehaviour
{
    void Update()
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
