using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HappinessEffectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject BarIns;
    public Transform Root;
    public GameObject Panel;
    private int m_count;
    public void Refresh(SolidMiddleware<Int> happinessTotal)
    {
        for (int i = 0; i < Root.childCount; i++)
        {
            Destroy(Root.GetChild(i).gameObject);
        }
        Display(happinessTotal);
        
    }
    private void Display(SolidMiddleware<Int> happinessTotal)
    {
        m_count = happinessTotal.CPUs.Count;
        for (int i = 0; i < happinessTotal.CPUs.Count; i++)
        {
            var bar = Instantiate(BarIns, Root);
            var cpu = happinessTotal.CPUs[i];
            if (cpu.Addition.m_value < 0)
            {
                var texts = bar.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].SetText(cpu.name);
                texts[1].SetText("<color=red>" + cpu.Addition.m_value + "</color>");
            }
            else if (cpu.Addition.m_value > 0)
            {
                var texts = bar.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].SetText(cpu.name);
                texts[1].SetText("<color=green>" + "+" + cpu.Addition.m_value + "</color>");
            }
            bar.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(m_count >=1)
            Panel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
