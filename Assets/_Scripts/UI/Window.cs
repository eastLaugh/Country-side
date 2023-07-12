using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
public class Window : MonoBehaviour
{
    public bool AllowClose;

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
}
