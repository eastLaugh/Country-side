using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    
    [SerializeField] Button startBtn;
    [SerializeField] Button randomBtn;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField saveNameInput;
    [SerializeField] GameObject Overlay;
    void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            if(saveNameInput.text != "" && nameInput.text != "")
            {
                Overlay.SetActive(true);
                StartCoroutine(Creat(saveNameInput.text));
            }        
        });
        randomBtn.onClick.AddListener(() =>
        {
            string[] adjectives = { "��ȵ�", "�ϻ۵�", "�󵨵�", "ʷʫ��", "��η��", "�Ի͵�", "Ӣ�۵�", "����˼��֮", "ǿ���", "ǿ����", "��Ч��", "ͻ���Ե�" };
            string[] nouns = { "����", "̽��", "׷��", "�ó�", "����", "Զ��", "��ҵ", "Ͷ��", "�¼�" };
            saveNameInput.text = adjectives[Random.Range(0, adjectives.Length)] + nouns[Random.Range(0, nouns.Length)];
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Creat(string fileName)
    {
        yield return null;
        //var fileName = Path.GetFileName(filePath);
        GameManager.current.NewGame(fileName);
        gameObject.SetActive(false);
        Overlay.SetActive(false);
        AudioSystem.current.SwitchMusic(SoundName.GameMusic);
        yield break;
    }
}
