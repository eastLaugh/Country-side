using UnityEngine;
using DG.Tweening;
public class SlotRender : MonoBehaviour , IClickable{
    public Slot slot;

    public void OnClick()
    {
        transform.position = new Vector3(transform.position.x,transform.position.y+0.3f,transform.position.z);
    }

    public void UnClick()
    {
        transform.position = new Vector3(transform.position.x,transform.position.y-0.3f,transform.position.z);
    }

    private void Awake() {

        
    }
    private void Start() {
        
    }
}