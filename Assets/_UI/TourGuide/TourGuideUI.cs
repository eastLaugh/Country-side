using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourGuideUI : MonoBehaviour
{
    [SerializeField] private Button BtnClose;
    [SerializeField] private GameObject Window;
    void Start()
    {
        BtnClose.onClick.AddListener(() =>
        {
            EventHandler.CallInitSoundEffect(SoundName.BtnClick);
            Window.SetActive(false);
        });
        EventHandler.ToAdiminstration += CallVillageHead;
    }
    private void CallVillageHead()
    {
        Window.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
