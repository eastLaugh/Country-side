using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
public class Window : MonoBehaviour
{
    public bool AllowClose;

    [ReadOnly]
    public TextMeshProUGUI Title;
    /// <summary>
    /// 关闭窗口时调用
    /// </summary>
    public UnityEvent OnClose;

    public void Close(){
        if(AllowClose){
            Destroy(gameObject);
            OnClose.Invoke();
        }else{
            GetComponent<RectTransform>().DOShakeAnchorPos(0.2f, 10, 100, 90, false, true);
        }
    }

    public void SetTitle(string text){
        Title.SetText(text);
    }
}
