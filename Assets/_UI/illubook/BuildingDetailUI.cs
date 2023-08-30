using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//�Զ���̳���MonoBehaviour��BuildingDetailUI�࣬����ʵ����Ʒ��ϸ��Ϣ��UI����
public class BuildingDetailUI : MonoBehaviour
{
    //������Ʒ�����ԣ����ơ�������ͼ��
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI functionText;
    public TextMeshProUGUI requirementText;
    public Image icon;

    //������Ʒ��Building���жϲ���ʾ����Ʒ�����ơ�������ͼ��
    public void Display(BuildingDetails Building)
    {
        nameText.text = Building.name;
        descriptionText.text = Building.description;
        functionText.text = Building.function;
        requirementText.text = Building.requirement;
        icon.sprite = Building.icon;
        
        //��Ʒͼ�겻����ʱ�����ظ�ͼ��
        if (Building.icon != null)
            icon.gameObject.SetActive(true);
    }

    //�жϲ������ʾ����Ʒ��Ϣ�������Ʒͼ�겻���ڣ����������ͼ��
    public void Clear()
    {
        nameText.text = null;
        descriptionText.text = null;
        functionText.text = null;
        requirementText.text = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
    }
}
