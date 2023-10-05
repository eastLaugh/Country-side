using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image powerFill;
    [SerializeField] private GameObject powerText;

    public void Show(float powerPercentage)
    {
        powerFill.fillAmount = powerPercentage;
        powerText.GetComponentInChildren<TextMeshProUGUI>().text = "绿色能源占比" + (powerPercentage * 100).ToString("F0")+"%";
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        powerText.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        powerText.SetActive(true);
    }
}
