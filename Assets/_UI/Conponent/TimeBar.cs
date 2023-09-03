using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TimeBar : MonoBehaviour
{
    public TimeController timeController;
    [SerializeField] TextMeshProUGUI displayDate;

    public void Initialize(TimeController timeController)
    {
        this.timeController = timeController;
    }

    public void Play()
    {
        timeController.Play();
    }
    public void Stop()
    {
        timeController.Stop();
    }
    public void FastForward()
    {
        timeController.FastForward();
    }

    void Update()
    {
        if(GameManager.current.CurrentSate == GameManager.GameState.Playing.ToString())
            displayDate.text = GameManager.current.map.dateTime.Date.ToString("yyyy/MM/dd");
    }
}
