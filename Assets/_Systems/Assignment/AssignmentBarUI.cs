using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AssignmentBarUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public void UpdateBar(string name)
    {
        nameText.text = name;
    }
    public void DestroyBar()
    {
        nameText.transform.DOScale(1.3f, 1.9f);
        nameText.color = Color.green;
    }
}
