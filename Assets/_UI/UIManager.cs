using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    TimeSystem timeSystem;
    illuBookSystem illuBookSystem;
    [SerializeField]
    TimeBar timeBar;
    [SerializeField]
    illuBookUI illuBookUI;
    [SerializeField]
    EventUISystem eventUI;

    public void Initialize(TimeSystem timeSystem,illuBookSystem illuBookSystem)
    {
        this.illuBookSystem = illuBookSystem;
        this.timeSystem = timeSystem;
        timeBar.Initialize(timeSystem);
        illuBookUI.Initialize(illuBookSystem,timeSystem);
        eventUI.Initialize(timeSystem);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
