using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    TimeSystem timeSystem;
    illuBookSystem illuBookSystem;
    [SerializeField]
    illuBookUI illuBookUI;
    [SerializeField]
    EventUISystem eventUI;

    public void Initialize(TimeSystem timeSystem,illuBookSystem illuBookSystem)
    {
        this.illuBookSystem = illuBookSystem;
        this.timeSystem = timeSystem;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
