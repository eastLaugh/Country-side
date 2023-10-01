using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestChatGpt : MonoBehaviour
{

    public ChatGPTWrapper.ChatGPTConversation chatGPTConversation;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Get(string sth){
        Debug.Log(sth);
    }
    public string sth;
    [NaughtyAttributes.Button]
    void SaySth(){
        chatGPTConversation.SendToChatGPT(sth);
    }
}
