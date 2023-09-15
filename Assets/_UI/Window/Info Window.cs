using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoWindow : MonoBehaviour
{
    public static InfoWindow Ovary;  //卵巢，用于产卵的、在场景中的预制件
    public TextMeshProUGUI Text;
    private void Awake()
    {
        if (Ovary == null)
        {
            Ovary = this;
            gameObject.SetActive(false);
        }
    }

    public static InfoWindow Create(string text)
    {
        GameObject egg = Instantiate(Ovary.gameObject, Ovary.transform.position, Quaternion.identity, Ovary.transform.parent);
        egg.SetActive(true);
        InfoWindow infoWindow = egg.GetComponentInChildren<InfoWindow>();
        infoWindow.Text.SetText(text);
        return infoWindow;
    }

    public static InfoWindow Create(object obj) => Create(obj.ToString());
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
