using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AssignmentBarUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI prizeText;
    public void UpdateBar(string name,string prize)
    {
        nameText.text = name;
        prizeText.text = "½±Àø£º" + prize;
    }
    public void DestroyBar()
    {
        nameText.transform.DOScale(1.3f, 1.9f);
        nameText.color = Color.green;
    }
}
