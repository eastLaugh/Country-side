using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TimeBar : MonoBehaviour
{
    private TimeSystem timeSystem;
    [SerializeField] TextMeshProUGUI displayDate;

    public void Initialize(TimeSystem timeSystem)
    {
        this.timeSystem = timeSystem;
    }

    public void Play()
    {
        timeSystem.Play();
    }
    public void Stop()
    {
        timeSystem.Stop();
    }
    public void FastForward()
    {
        timeSystem.FastForward();
    }

    void Update()
    {
        if(GameManager.current.CurrentSate == GameManager.GameState.Playing.ToString())
            displayDate.text = GameManager.current.map.dateTime.Date.ToString("yyyy/MM/dd");
    }
}
