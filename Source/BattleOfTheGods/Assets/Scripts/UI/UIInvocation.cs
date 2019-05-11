using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIInvocation : MonoBehaviour
{
    [SerializeField] Text textButtonInvoke = null;
    [SerializeField] Button buttonInvoke = null;

    [SerializeField] GameObject panelInvoke = null;
    [SerializeField] Image imagePicture = null;
    [SerializeField] Text textName = null;
    [SerializeField] Image imageType = null;
    [SerializeField] Text textHp = null;
    [SerializeField] Text textAtk = null;
    [SerializeField] Text textAtkrange = null;


    List<int> databaseId;

    void Start()
    {
        databaseId = DataManager.singleton.GetDatabase().Select(x => x.Id).ToList();

        if (databaseId.Count == 0)
        {
            buttonInvoke.interactable = false;
            textButtonInvoke.text = "you already own all the characters";
        }
    }

    public void InvokeCharacter()
    {
        panelInvoke.SetActive(true);
        int idChar = databaseId[Random.Range(0, databaseId.Count)];
        DataManager.singleton.AddInCollection(idChar);
        
        CharacterData data = DataManager.singleton.GetData(idChar);

        imagePicture.sprite = data.Picture;
        textName.text = data.name;
        textHp.text = data.Hp.ToString();
        textAtk.text = data.Attack.ToString();
        textAtkrange.text = data.AttackRange.ToString();

        switch (data.CharacterType)
        {
            case CharacterType.None:
                imageType.color = Color.gray;
                break;
            case CharacterType.Red:
                imageType.color = Color.red;
                break;
            case CharacterType.Blue:
                imageType.color = Color.blue;
                break;
            case CharacterType.Green:
                imageType.color = Color.green;
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        panelInvoke.SetActive(false);
    }
}
