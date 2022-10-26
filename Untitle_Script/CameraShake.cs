using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeAmount = 0.04f;

    private Vector3 initialPosition;
    private Vector3 endPosition;

    private float div = 1f;
    [SerializeField] private float ShakeTime;

    private void Start()
    {
        initialPosition = gameObject.transform.position;
        endPosition = new Vector3(5.5f, 3.5f, -10f);
    }

    private void Update()
    {
        if (ShakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * (ShakeAmount / div) + endPosition;
            
            ShakeTime -= Time.deltaTime;

            if(ShakeTime < 0.2f)
            {
                ShakeTime = 0f;
                transform.position = initialPosition;
            }
        }
    }

    public void ShakeForTimeAdd(float time)
    {
        ShakeTime = time;
    }

    public void ShakeForTime(float time)
    {
        ShakeTime = time;
    }

    public void SetDiv(float div)
    {
        this.div = div;
    }
}
