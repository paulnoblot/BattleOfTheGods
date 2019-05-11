using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    public static UIGame singleton;

    [Header("Character Info")]
    [SerializeField] GameObject panelCharacterInfo = null; 
    [SerializeField] Image imagePicture = null;
    [SerializeField] Text textName = null;
    [SerializeField] Text textHp = null;
    [SerializeField] Text textAtk = null;
    [SerializeField] Text textAtkRange = null;
    [SerializeField] Text textLvl = null;
    [SerializeField] Text textExp = null;

    [Header("Character Lvl Up")]
    [SerializeField] GameObject panelLvlUp = null;
    [SerializeField] Image imageLvlUpPicture = null;
    [SerializeField] Text textLvlUpName = null;
    [SerializeField] Text textLvlUpHp = null;
    [SerializeField] Text textLvlUpAtk = null;
    [SerializeField] Text textLvlUpLvl = null;
    [SerializeField] AudioSource soundLevelUp = null;

    [Header("End Of Turn")]
    [SerializeField] GameObject panelEndOfTurn = null;
    [SerializeField] Text textEndOfTurn = null;

    [Header("End Of Stage")]
    [SerializeField] GameObject panelEndOfStage = null;
    [SerializeField] Text textEndOfStage = null;

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }

    private void Start()
    {
        TurnManager.singleton.endOfTurn.AddListener(EndOfTurn);
        TurnManager.singleton.endOfStage.AddListener(EndOfStage);
    }

    public void DisplayCharacterInfo(Character _character)
    {
        if (_character == null)
        {
            panelCharacterInfo.SetActive(false);
        }
        else
        {
            panelCharacterInfo.SetActive(true);
            imagePicture.sprite = _character.data.Picture;
            textName.text = _character.data.name;
            textHp.text = _character.hp + " / " + _character.maxHp;
            textAtk.text = _character.attack.ToString();
            textAtkRange.text = _character.data.AttackRange.ToString();
            textLvl.text = _character.lvl.ToString();
            textExp.text = _character.exp + " / " + CharacterProgression.GetExpNeededAtlvl(_character.lvl);
        }
    }

    public void DisplayCharacterUp(Character _character)
    {
        soundLevelUp.Play();

        int oldHp = _character.data.Hp + (int)((_character.lvl - 2) * _character.data.HpPerLvl);
        int oldAttack = _character.data.Attack + (int)((_character.lvl - 2) * _character.data.AttackPerLvl);

        imageLvlUpPicture.sprite = _character.data.Picture;
        textLvlUpName.text = _character.data.name;
        textLvlUpLvl.text = (_character.lvl - 1) +  " -> " + _character.lvl;
        textLvlUpHp.text = oldHp + " -> " + _character.maxHp;
        textLvlUpAtk.text = oldAttack + " -> " + _character.attack;

        panelLvlUp.SetActive(true);
    }


    void EndOfTurn(int _idNextPlayer)
    {
        if (panelEndOfTurn)
        {
            if (_idNextPlayer == 0)
            {
                textEndOfTurn.text = "YOUR TURN";
            }
            else
            {
                textEndOfTurn.text = "ENEMY TURN";
            }

            panelEndOfTurn.SetActive(true);
            Invoke("HidePanelEndOfTurn", 1f);
        }
    }

    void HidePanelEndOfTurn()
    {
        if (panelEndOfTurn)
            panelEndOfTurn.SetActive(false);
    }

    void EndOfStage(int _idWinner)
    {
        Destroy(panelEndOfTurn);
        textEndOfStage.text = _idWinner == 0 ? "YOU WIN" : "YOU LOOSE";
        panelEndOfStage.SetActive(true);
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void HidePanelLvlUp()
    {
        panelLvlUp.SetActive(false);
    }
}
