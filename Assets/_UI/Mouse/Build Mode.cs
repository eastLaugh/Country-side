using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : StateMachineBehaviour
{

    //是否进入建造模式
    public static bool hasEntered { get; private set; }
    public static event Action OnBuildModeEnter;
    public static event Action OnBuildModeExit;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasEntered = true;
        SlotRender.OnAnySlotClickedInBuildMode += OnAnySlotClickedInBuildMode;
        OnBuildModeEnter?.Invoke();
    }

    void OnAnySlotClickedInBuildMode(SlotRender render)
    {
        if (BuildingWindow.TryGetSelectedTypeConfig(out Type selectedType, out MapObjectDatabase.Config config))
        {

            Vector2 delta = config.Size;

            bool canBuild = true;
            Action<Slot.MapObject> ApplyTo = null;
            BuildingWindow.Foreach(render.slot.position, config.Size, (x, y) =>
            {
                if (Slot.MapObject.CanBeInjected(render.slot.map[x, y], selectedType))
                {
                    if (x != render.slot.position.x || y != render.slot.position.y)
                    {
                        ApplyTo += host =>
                        {
                            new MapObjects.PlaceHolder(host).Inject(render.slot.map[x, y], direction: BuildingWindow.selectedDirection);
                        };
                    }
                }
                else
                {
                    canBuild = false;
                }
            });

            if (canBuild)
            {
                Slot.MapObject mapObject = Activator.CreateInstance(selectedType) as Slot.MapObject;
                ApplyTo?.Invoke(mapObject);
                mapObject.Inject(render.slot, direction: BuildingWindow.selectedDirection);
                //render.Refresh();
                EventHandler.CallInitSoundEffect(SoundName.Costruct);
            }
            else
            {
                Debug.LogWarning("占位符不能在此处创建");
                EventHandler.CallInitSoundEffect(SoundName.WrongPlace);
            }
        }
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasEntered = false;
        SlotRender.OnAnySlotClickedInBuildMode -= OnAnySlotClickedInBuildMode;
        OnBuildModeExit?.Invoke();
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
