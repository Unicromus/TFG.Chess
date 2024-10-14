using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockPlusButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.PlusTimer();
    }

}
