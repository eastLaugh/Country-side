using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnWindow : MonoBehaviour
{
    [SerializeField] Button BtnYes;
    [SerializeField] Button BtnNo;
    [SerializeField] GameObject Overlay;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject window;
    [SerializeField] TimeController timeController;
    GameManager gameManager  => GameManager.current;
    private void Start()
    {
        BtnYes.onClick.AddListener(() =>
        {
            timeController.Continue();
            Overlay.SetActive(true);
            startMenu.SetActive(true);
            StartCoroutine(AutoSave());
        });
        BtnNo.onClick.AddListener(() =>
        {
            timeController.Continue();
            gameObject.SetActive(false);
        });
    }

    IEnumerator AutoSave()
    {
        yield return null;
        window.SetActive(false);
        gameManager.AutoSave();
        Overlay.SetActive(false);
        AudioSystem.current.SwitchMusic(SoundName.MenuMusic);
        yield break;

    }
}
