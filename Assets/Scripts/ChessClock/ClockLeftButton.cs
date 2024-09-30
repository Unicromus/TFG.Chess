using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockLeftButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.BlackSwap();
    }

}
