using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockResetButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.ResetTimerAndBoard();
    }

}
