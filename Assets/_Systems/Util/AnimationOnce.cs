using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnce : MonoBehaviour
{
    public void End()
    {
        var animator = GetComponent<Animator>();
        animator.enabled = false;
        gameObject.SetActive(false);
        AudioSystem.current.SwitchMusic(SoundName.MenuMusic);
    }
}
