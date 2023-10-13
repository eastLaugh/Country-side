using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
    [SerializeField] GameObject Panel;
    [SerializeField] Button BtnClose;
    [SerializeField] TimeController timeController;
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;

        // Debug.Log("OnEable");
    }
    private void OnDisable()
    {
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        isMaploaded = false;
        BtnClose.onClick.RemoveAllListeners();
    }
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded = true;
        if(!map.isPrefaceDone)
        {
            Panel.SetActive(true);
            timeController.Play();
            timeController.Pause();
        }
        else
        {
            timeController.Play();
        }
        BtnClose.onClick.AddListener(() =>
        {
            Panel.SetActive(false);
            EventHandler.CallInitSoundEffect(SoundName.BtnClick3);
            map.isPrefaceDone = true;
            timeController.Continue();
        });
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);
        
    }
    void Start()
    {

    }

    
}
