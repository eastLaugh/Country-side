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
    public TextMeshProUGUI MainText;
    public Image icon;

    //������Ʒ��Building���жϲ���ʾ����Ʒ�����ơ�������ͼ��
    public void Display(BuildingDetails Building)
    {
        nameText.text = Building.chineseName;
        MainText.text = "����ԭ��" + Building.description + "\n\n" + "��������:" + Building.requirement + "\n\n" + "�������ܣ�" + Building.function;
        icon.sprite = Building.icon;
        
        //��Ʒͼ�겻����ʱ�����ظ�ͼ��
        if (Building.icon != null)
            icon.gameObject.SetActive(true);
    }

    //�жϲ������ʾ����Ʒ��Ϣ�������Ʒͼ�겻���ڣ����������ͼ��
    public void Clear()
    {
        nameText.text = null;
        MainText.text = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
    }
}
