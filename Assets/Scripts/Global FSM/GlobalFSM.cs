using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class GlobalFSM : MonoBehaviour
{
    private static GlobalFSM one;
    Dictionary<IFSM, FSMinfo> dict = new();

    private class FSMinfo
    {
        public string current;
        //public string[] states;

    }
    private void Awake()
    {
        one = this;
    }
    static string MethodName = "_{0}{1}";
    void Update()
    {
        foreach (var keyValuePair in dict)
        {
            IFSM register = keyValuePair.Key;
            if (register.enabled)
            {
                FSMinfo info = keyValuePair.Value;
                ConductMethod(register, info.current, "On");
            }
        }
    }

    void ConductMethod(IFSM obj, string state, string methodPrefix)
    {
        obj.GetType().GetMethod(string.Format(MethodName, methodPrefix, state), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?.Invoke(obj, null);
    }
    public interface IFSM
    {
        IFSM FSM { get; }
        bool enabled { get; }
        sealed void ChangeState(string state)
        {
            if (GlobalFSM.one.dict.ContainsKey(this))
            {
                GlobalFSM.one.ConductMethod(this, GlobalFSM.one.dict[this].current, "Exit");
                GlobalFSM.one.dict[this].current = state;
                GlobalFSM.one.ConductMethod(this, state, "Enter");
            }
            else
            {
                GlobalFSM.one.dict.Add(this, new FSMinfo { current = state});
                GlobalFSM.one.ConductMethod(this, state, "Enter");

            }


        }
    }
}

