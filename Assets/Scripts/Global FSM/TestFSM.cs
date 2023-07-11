using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestFSM : MonoBehaviour,GlobalFSM.IFSM
{
    public GlobalFSM.IFSM FSM => this;

    public void Awake() {
        FSM.ChangeState("Idle");
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
        FSM.ChangeState("Walk");
    }
    void _OnWalk(){
        Debug.Log("Walk");
        FSM.ChangeState("Run");
    }

    void _OnRun(){
        Debug.Log("Run");
        FSM.ChangeState("Idle");
    }
}
