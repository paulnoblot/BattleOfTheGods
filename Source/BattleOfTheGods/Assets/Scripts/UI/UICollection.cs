using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollection : MonoBehaviour
{
    [SerializeField] GameObject buttonModel = null;
    [SerializeField] Transform collectionContent = null;
    [SerializeField] Transform teamContent = null;

    [Header("CharacterInfo")]
    [SerializeField] GameObject panelCharacterInfo = null;
    [SerializeField] Image characterPicture = null;
    [SerializeField] Image characterType = null;
    [SerializeField] Text characterName = null;
    [SerializeField] Text characterLvl = null;
    [SerializeField] Text characterExp = null;
    [SerializeField] Text characterHp = null;
    [SerializeField] Text characterAtk = null;
    [SerializeField] Text characterAtkRange = null;

    void OnEnable()
    {
        // load collection
        foreach (CharacterProgression item in DataManager.singleton.GetCollection())
        {
            CharacterData data = DataManager.singleton.GetData(item.idCharacter);
            GameObject newButton;
            newButton = Instantiate(buttonModel, collectionContent);
            newButton.GetComponent<ButtonCollection>().SetData(item, data);
            newButton.GetComponent<Button>().onClick.AddListener(() => OnButtonCollectionClick(newButton.GetComponent<ButtonCollection>()));
            newButton.SetActive(true);
        }

        // load team
        foreach (int id in DataManager.singleton.GetTeam())
        {
            if (id != 0)
            {
                GameObject newButton;
                CharacterProgression progress = DataManager.singleton.GetProgression(id);
                newButton = Instantiate(buttonModel, teamContent);
                newButton.name = progress.idCollection.ToString();
                newButton.GetComponent<ButtonCollection>().SetData(progress, DataManager.singleton.GetData(progress.idCharacter));
                newButton.GetComponent<Button>().onClick.AddListener(() => OnButtonTeamClick(newButton.GetComponent<ButtonCollection>()));
                newButton.SetActive(true);

                foreach (Transform item in collectionContent)
                {
                    if (item.GetComponent<ButtonCollection>().GetProgress().idCollection == progress.idCollection)
                    {
                        item.GetComponent<ButtonCollection>().DisplayCheckmark(true);
                        continue;
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        panelCharacterInfo.SetActive(false);
        foreach (Transform item in collectionContent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in teamContent)
        {
            Destroy(item.gameObject);
        }
    }

    void OnButtonCollectionClick(ButtonCollection _btn)
    {
        DisplayCharacterInfo(_btn);

        foreach (Transform item in teamContent)
        {
            if (item.GetComponent<ButtonCollection>().GetProgress().idCollection == _btn.GetProgress().idCollection)
            {
                _btn.DisplayCheckmark(false);
                Destroy(item.gameObject);
                return;
            }
        }

        if (teamContent.childCount >= 4)
            return;

        GameObject newButton;
        newButton = Instantiate(buttonModel, teamContent);
        newButton.name = _btn.GetProgress().idCollection.ToString();
        newButton.GetComponent<ButtonCollection>().SetData(_btn.GetProgress(), _btn.GetData());
        newButton.GetComponent<Button>().onClick.AddListener(() => OnButtonTeamClick(newButton.GetComponent<ButtonCollection>()));
        newButton.SetActive(true);

        _btn.DisplayCheckmark(true);
    }

    void OnButtonTeamClick(ButtonCollection _btn)
    {
        DisplayCharacterInfo(_btn);
    }

    public void SaveTeam()
    {
        int[] idColls = new int[4];
        
        for (int i = 0; i < 4; i++)
        {
            if (teamContent.childCount > i)
                idColls[i] = teamContent.GetChild(i).GetComponent<ButtonCollection>().GetProgress().idCollection;
            else
                idColls[i] = 0;
        }

        DataManager.singleton.SaveTeam(idColls);
    }

    void DisplayCharacterInfo(ButtonCollection _btn)
    {
        characterPicture.sprite = _btn.GetData().Picture;

        switch (_btn.GetData().CharacterType)
        {
            case CharacterType.None:
                characterType.color = Color.gray;
                break;
            case CharacterType.Red:
                characterType.color = Color.red;
                break;
            case CharacterType.Blue:
                characterType.color = Color.blue;
                break;
            case CharacterType.Green:
                characterType.color = Color.green;
                break;
            default:
                break;
        }

        characterName.text = _btn.GetData().name;
        characterLvl.text = _btn.GetProgress().lvl.ToString();
        characterExp.text = _btn.GetProgress().exp + " / " + CharacterProgression.GetExpNeededAtlvl(_btn.GetProgress().lvl);
        characterHp.text = _btn.GetData().GetHpAtLvl(_btn.GetProgress().lvl).ToString();
        characterAtk.text = _btn.GetData().GetAttackAtLvl(_btn.GetProgress().lvl).ToString();
        characterAtkRange.text = _btn.GetData().AttackRange.ToString();

        panelCharacterInfo.SetActive(true);
    }
}
