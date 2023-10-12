using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOut : MonoBehaviour
{
    [SerializeField] GameObject hint;
    [SerializeField] GameObject startMenu;
    private bool canExit = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!canExit) { return; }
        if(Input.anyKeyDown)
        {
            startMenu.SetActive(true);
            canExit = false;
            gameObject.SetActive(false);
            hint.SetActive(false);
            AudioSystem.current.SwitchMusic(SoundName.MenuMusic);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick3);
        }
    }

    public void Exit()
    {
        canExit = true;
        hint.SetActive(true);
    }
}
