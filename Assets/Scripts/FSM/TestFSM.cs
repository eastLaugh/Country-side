using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestFSM : MonoBehaviour,GlobalFSM.IFSM
{
    public GlobalFSM.IFSM api => this;

    public void Awake() {
        api.ChangeState("Idle");
    }

    private void Start() {
        
    }
    void _EnterIdle(){
        Debug.Log("EnterIdle");
    }
    void _ExitIdle(){
        Debug.Log("ExitIdle");
    }
    void _EnterWalk(){
        Debug.Log("EnterWalk");
    }
    void _ExitWalk(){
        Debug.Log("ExitWalk");
    }
    void _EnterRun(){
        Debug.Log("EnterRun");
    }
    void _ExitRun(){
        Debug.Log("ExitRun");
    }
    void _OnIdle(){
        Debug.Log("Idle");
        api.ChangeState("Walk");
    }
    void _OnWalk(){
        Debug.Log("Walk");
        api.ChangeState("Run");
    }

    void _OnRun(){
        Debug.Log("Run");
        api.ChangeState("Idle");
    }
}
