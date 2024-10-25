using UnityEngine;

public class ClockStopButton : MonoBehaviour
{

    [SerializeField] private ChessClock chessClock;

    private void OnMouseUpAsButton()
    {
        chessClock.StopTimer();
    }

}
