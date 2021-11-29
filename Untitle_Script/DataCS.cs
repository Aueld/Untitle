using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCS
{
    private static DataCS Instance;

    public static DataCS instance
    {
        get
        {
            if (instance == null)
                Instance = new DataCS();
            return instance;
        }
    }
    public static int mainLevel = 1;
    public static int mainFood = 100;
    public static int mainScore = 0;
}
