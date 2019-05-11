using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCollection : MonoBehaviour
{
    CharacterProgression progress = null;
    CharacterData data = null;

    [SerializeField] Image picture = null;
    [SerializeField] Text textLvl = null;
    [SerializeField] GameObject checkmarck = null;
    
    public void SetData(CharacterProgression _progress, CharacterData _data)
    {
        progress = _progress;
        data = _data;
        textLvl.text = _progress.lvl.ToString();
        picture.sprite = data.Picture;
    }

    public CharacterData GetData()
    {
        return data;
    }

    public CharacterProgression GetProgress()
    {
        return progress;
    }

    public void DisplayCheckmark(bool _b)
    {
        checkmarck.gameObject.SetActive(_b);
    }
}
