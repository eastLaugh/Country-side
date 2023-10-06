using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;     //����Unity�ٷ��Ļ�����ܣ����Է��㴦��UI�Ļ����¼�������������ק������
using UnityEngine.UI;
//using UnityEngine.UIElements;


//�Զ���VGF.Inventory���ṩ���������ͣ���뿪ʱ�����¼��Ľӿ�

    //�̳ж�����෽�����ʵ�ֶ��ֹ���
public class BarUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    //���屳����Ʒ�Ķ���ж�����ʾָ��
    private BuildingDetails BuildingDetails;
    public bool Selected = false;
    public Image image;
    public Image Mask;
    public TextMeshProUGUI BuildingName;
    public BuildingType slotType;
    public BuildingDetailUI BuildingDetailUI;
    public int slotIndex;
    
    //��image��ʼ��Ϊ��Slot�������Image���
    private void Start()
    {
        //BuildingDetails = new BuildingDetails();
    }

    //�ɼ�����Ի��Զ����ˢ������
    void Update()
    {
        //
    }
    
    /// <summary>
    /// ����Bar
    /// </summary>
    public void UpdateBuildingBar(BuildingDetails Building)     //���²���ȡ��Ʒ�����ƺ�����
    {
        BuildingDetails = Building;
        BuildingName.text = BuildingDetails.chineseName;
        image.sprite = null;
        if(BuildingDetails.unclock)
        {
            Mask.gameObject.SetActive(false); 
        }
        else
        {
            Mask.gameObject.SetActive(true);
        }
    }
    
    //��BuildingDetils�ÿգ�������һ����Ʒ����ʾ
    private void OnDestroy()
    {
        BuildingDetails = null;
    }

    //�ڱ�����λ�����ʱ����������Ʒ��BuildingDetails���󴫵ݸ�Display()����
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!BuildingDetails.unclock) return;
        EventHandler.CallInitSoundEffect(SoundName.BtnClick2);
        Selected = true;
        var color = image.color;
        color.a = 0.5f;                     //����image��͸��������Ϊ50%
        image.color = color;
        BuildingDetailUI.Display(BuildingDetails);

    }

    //�������ͣ�ڱ�����λʱ����������ͼ����ʾ��͸���ȣ�ͻ����ʾ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!BuildingDetails.unclock) return;
        if (!Selected)
        {
            var color = image.color;
            color.a = 0.8f;                 //����image��͸����Ϊ80%
            image.color = color;
        }
    }

    //�����Ӳ�λ���ƿ���ʱ�򴥷�������ͼ���͸���ȣ��ָ�������ʾ������
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!BuildingDetails.unclock) return;
        if (!Selected)
        {
            var color = image.color;
            color.a = 1f;                   //����image��͸����Ϊ100%������͸��
            image.color = color;
        }
    }
}

