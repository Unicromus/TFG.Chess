using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockRightButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.WhiteSwap();
    }

}
