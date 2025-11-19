using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using ChatGPTWrapper;
using UnityEngine.UI;
public class ChatWindow : MonoBehaviour
{
    public static ChatWindow Ovary;

    Person person;
    public ChatGPTConversation gpt;
    public TMP_InputField inputField;

    public GameObject chatMessagePrefab;

    public Transform Content;


    void Awake()
    {
        if (Ovary == null)
        {
            Ovary = this;
            gameObject.SetActive(false);
        }


    }
    // Start is called before the first frame update
    void Start()
    {

        inputField.onSubmit.AddListener(t0 =>
        {
            inputField.SetTextWithoutNotify("");
            // inputField.ActivateInputField();
            Push(t0, true);

            Reset();
            gpt.SendToChatGPT(t0);
            inputField.interactable = false;

        });

        if (person != null)
        {
            // if (GameManager.DebugMode)
            //     Push(initial, true);

            gpt.enabled = true;
            gpt.chatGPTResponse.AddListener(res =>
            {
                Push(res);
                inputField.interactable = true;
            });
        }
    }

    void Reset()
    {
        string initial2 = person.GetInitialPrompt();
        gpt.ResetChat(initial2);
        Debug.Log(initial2);

    }
    void Push(string text, bool player = false)
    {
        var prefab = Instantiate(chatMessagePrefab, Content);
        prefab.SetActive(true);
        prefab.GetComponentInChildren<TextMeshProUGUI>().text = text;
        prefab.name = text;

        if (player)
        {
            prefab.GetComponentInChildren<Button>().interactable = false;
        }
    }

    private static Dictionary<Person, ChatWindow> ChatWindows = new();
    public static void OpenOrCreate(Person p)
    {
        if (ChatWindows.TryGetValue(p, out ChatWindow window))
        {
            window.gameObject.SetActive(true);
            window.transform.DOShakePosition(0.5f);
        }
        else
        {
            ChatWindow son = Instantiate(Ovary, Ovary.transform.parent);
            son.gameObject.name = "Chat : " + p.name;
            son.gameObject.SetActive(true);
            son.GetComponent<ChatWindow>().person = p;

            ChatWindows.Add(p, son);
        }
    }

    public static void Close(Person p)
    {
        if (ChatWindows.TryGetValue(p, out ChatWindow window))
        {
            window.gameObject.SetActive(false);
        }
    }
}
