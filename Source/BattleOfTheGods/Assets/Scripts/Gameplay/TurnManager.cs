using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager singleton;

    public int playerTurn = 0;

    public IntEvent endOfTurn = new IntEvent();
    public IntEvent endOfStage = new IntEvent();

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

    List<Character> firstTeam; 
    List<Character> secondTeam;

    // Start is called before the first frame update
    void Start()
    {
        firstTeam = FindObjectsOfType<Character>().Where(x => x.idTeam == 0).ToList();
        secondTeam = FindObjectsOfType<Character>().Where(x => x.idTeam == 1).ToList();
    }


    public void RemoveCharacter(Character _character)
    {
        if (_character.idTeam == 0)
        {
            firstTeam.Remove(_character);
            if (firstTeam.Count == 0)
            {
                EndOfStage(1);
            }
        }
        else
        {
            secondTeam.Remove(_character);
            if (secondTeam.Count == 0)
            {
                EndOfStage(0);
            }
        }
    }

    public void CharacterEndPlay(Character _character)
    {
        if (_character.idTeam == 0)
        {
            foreach (var item in firstTeam)
                if (item.haveAttack == false)
                    return;
            EndOfTurn();
        }
        else
        {
            foreach (var item in secondTeam)
                if (item.haveAttack == false)
                    return;
            EndOfTurn();
        }
    }

    public void EndOfTurn()
    {
        foreach (var item in firstTeam)
        {
            item.ResetTurn();
        }
        foreach (var item in secondTeam)
        {
            item.ResetTurn();
        }
        playerTurn = playerTurn == 0 ? 1 : 0;
        endOfTurn.Invoke(playerTurn);
    }

    void EndOfStage(int _idWinner)
    {
        endOfStage.Invoke(_idWinner);
    }
}
