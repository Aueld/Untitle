using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerScript : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private bool checkTouch = false;
    private Image img;
    private Touch tempTouch;
    private Vector2 nowPosition;

    private void Start()
    {
        img = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                tempTouch = Input.GetTouch(i);
                if (tempTouch.phase == TouchPhase.Began)
                {
                    nowPosition = Camera.main.ScreenToWorldPoint(tempTouch.position);
                    checkTouch = true;
                    img.transform.position = nowPosition;

                    break;
                }
            }
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    nowPosition = Camera.main.ScreenToWorldPoint(tempTouch.position);
        //    checkTouch = true;
            
        //}
        if (checkTouch)
        {
            img.transform.position = nowPosition;
            Debug.Log(img.transform.position);
            checkTouch = false;
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) // 터치하고 있을때
    {
        //if(!checkTouch)
        //    nowPosition = ped.position;
        checkTouch = true;
    }

    public virtual void OnPointerUp(PointerEventData ped) // 터치 안할때
    {
        checkTouch = false;
    }
}
