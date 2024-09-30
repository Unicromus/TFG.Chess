using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockStartButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.StartTimer();
    }

}
