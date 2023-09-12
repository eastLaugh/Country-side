using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAssignment
{
    public string name;
    public bool unlock = false;
    public bool finished  = false;
    public string hint;
    private Func<bool> checkFunc;
    private Action onAssignmentFinished;
    public BasicAssignment(string name,string hint,Func<bool> func,Action action)
    {
        this.name = name;
        this.hint = hint;
        this.checkFunc = func;
        this.onAssignmentFinished = action;
    }
    public bool Check()
    {
        if (checkFunc != null)
        {
            return checkFunc();
        }
        return false;
    }
    public void Finish()
    {
        onAssignmentFinished?.Invoke();
        finished = true;
    }
}