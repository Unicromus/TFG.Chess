using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockStopButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.StopTimer();
    }

}
