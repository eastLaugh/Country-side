using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InfoWindow infoWindow = InfoWindow.Create("你已进入建造模式\n点击右下角×以退出");
        infoWindow.GetComponent<Window>().OnClose.AddListener(() =>
        {
            animator.SetTrigger("BuildEnd");
        });

        SlotRender.OnSlotSelected += OnSlotSelected;
    }

    void OnSlotSelected(SlotRender slotRender)
    {
        Type selectiveType = BuildingWindow.SelectiveType;
        Slot.MapObject mapObject = Activator.CreateInstance(selectiveType) as Slot.MapObject;
        mapObject.Inject(slotRender.slot);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SlotRender.OnSlotSelected -= OnSlotSelected;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
