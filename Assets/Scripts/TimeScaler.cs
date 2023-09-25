using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public void ScaleTime(float value)
    {
        Time.timeScale = value;
    }
}
