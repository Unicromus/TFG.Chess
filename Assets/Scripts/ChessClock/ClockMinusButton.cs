using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMinusButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.MinusTimer();
    }

}
