using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkPlayer : MonoBehaviour
{

    public TextAsset InkAsset;
    Story inkStory;
   
    // Start is called before the first frame update
    void Start()
    {
        inkStory = new Story(InkAsset.text);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Next")]
    void Next()
    {
        if (inkStory.canContinue)
        {
            string v = inkStory.Continue();
            Debug.Log(v); 
        }
    }
}
