using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicPool : MonoBehaviour
{
    // Start is called before the first frame update
    Queue<GameObject> soundQueue = new Queue<GameObject>();
    [SerializeField] GameObject SoundPrefab;
    private void OnEnable()
    {
        
        EventHandler.InitSoundEffect += InitSoundEffect;
    }
    private void OnDisable()
    {
        EventHandler.InitSoundEffect -= InitSoundEffect;
    }
    private void Start()
    {
        CreateSoundPool();
    }
    private void CreateSoundPool()
    {

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(SoundPrefab, transform);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }

    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)
            CreateSoundPool();
        return soundQueue.Dequeue();
    }

    private void InitSoundEffect(SoundName soundName)
    {
        var soundDetails = AudioSystem.current.SoundDetailsList_SO.GetSoundDetails(soundName);
        var obj = GetPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }

    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }
}
