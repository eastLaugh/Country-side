using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
//状态行为
public class CustomState
{
    private Action mOnEnter;
    private Action mOnUpdate;
    private Action mOnFixedUpdate;
    private Action mOnExit;

    //状态进入
    public CustomState OnEnter(Action act)
    {
        mOnEnter = act;
        return this;
    }
    public void Enter()
    {
        mOnEnter?.Invoke();
    }

    //状态更新
    public CustomState OnUpdate(Action act)
    {
        mOnUpdate = act;
        return this;
    }
    public void Update()
    {
        mOnUpdate?.Invoke();
    }

    //固定时间更新
    public CustomState OnFixedUpdate(Action act)
    {
        mOnFixedUpdate = act;
        return this;
    }
    public void FixedUpdate()
    {
        mOnFixedUpdate?.Invoke();
    }

    //状态退出
    public CustomState OnExit(Action act)
    {
        mOnExit = act;
        return this;
    }
    public void Exit()
    {
        mOnExit?.Invoke();
    }
}


//处理游戏中对象的状态变化
/*当状态机中的状态发生改变时，FSM会自动执行原来状态的Exit函数和新状态的Enter函数，以保证状态转移的正确性。*/
public class FSM<T> where T : struct, Enum
{
    //存储 FSM 中的所有状态
    public Dictionary<T, CustomState> States = new();

    //获取某个状态的 CustomState 对象
    public CustomState State(T t)
    {
        ////如果该状态不存在，则新建一个CustomState对象并返回
        if (!States.ContainsKey(t))
        {
            States.Add(t, new CustomState());
        }
        return States[t];
    }

    private CustomState CurrentCustomState;             //当前FSM的状态对应的CustomState对象
    private T? currentState = null;                     //当前FSM的状态，可以表示FSM当前没有状态
    public T? CurrentState { get => currentState; }     //获取当前FSM的状态


    //切换FSM的状态到t
    public void ChangeState(T t)
    {
        if (currentState.HasValue && currentState.GetValueOrDefault().Equals(t))
        {
            Debug.LogWarning("FSM已经处于状态" + t + ",自切换会重新执行一遍OnEnter");
        }
        if (States.TryGetValue(t, out var s))
        {
            CurrentCustomState?.Exit();
            currentState = t;
            CurrentCustomState = s;
            CurrentCustomState.Enter();
        }
        else
        {
            throw new ApplicationException("未注册状态，请使用State注册状态");
        }
    }

    //执行当前状态的FixedUpdate方法
    public void FixedUpdate()
    {
        CurrentCustomState?.FixedUpdate();
    }

    //执行当前状态的Update方法
    public void update()
    {
        CurrentCustomState?.Update();
    }

    //重置FSM，包括当前状态、CustomState对象和所有状态的注册信息
    public void Reset()
    {
        currentState = null;
        CurrentCustomState = null;
        States.Clear();
    }
}