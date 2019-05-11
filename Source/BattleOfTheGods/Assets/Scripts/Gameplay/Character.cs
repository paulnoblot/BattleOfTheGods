using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    public bool haveMove = false;
    public bool haveAttack = false;

    public CharacterData data;
    public CharacterProgression progress;
    public int hp;
    public int maxHp;
    public int attack;
    public int lvl;
    public int exp;

    [SerializeField] UnityEngine.UI.Image imageLifeBar = null;
    [SerializeField] UnityEngine.UI.Image imageType = null;
    [SerializeField] UnityEngine.UI.Text textHp = null;

    List<Vector2Int> enableMovePosition;
    List<Vector2Int> enableAttackPosition;

    public void SetTeam(int _idTeam)
    {
        idTeam = _idTeam;
        imageLifeBar.color = _idTeam == 0 ? Color.green : Color.red;
    }

    public void SetData(CharacterData _data, int _lvl = 1, int _exp = 0)
    {
        data = _data;
        
        hp = data.GetHpAtLvl(_lvl);
        maxHp = hp;
        attack = data.GetAttackAtLvl(_lvl);
        textHp.text = hp.ToString();
        lvl = _lvl;
        exp = _exp;
        GetComponent<SpriteRenderer>().sprite = data.Picture;

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

    public void SetData(CharacterData _data, CharacterProgression _progress)
    {
        progress = _progress;
        SetData(_data, _progress.lvl, _progress.lvl);
    }


    private void OnMouseDown()
    {
        Map.singleton.DisplayMovableCells(false);
        Map.singleton.DisplayAttackableCells(false);

        UIGame.singleton.DisplayCharacterInfo(this);

        if (TurnManager.singleton.playerTurn != idTeam)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sortingOrder = 20;

        if (!haveMove)
        {
            enableMovePosition = Map.singleton.GetMoveCells(this);
        }

        if (!haveAttack)
        {
            enableAttackPosition = Map.singleton.GetAttackCells(this);
        }
    }

    private void OnMouseDrag()
    {
        if (haveAttack || TurnManager.singleton.playerTurn != idTeam)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        Plane plane = new Plane(-Vector3.forward, 0);
        Vector3 v3 = Vector3.zero;

        if (plane.Raycast(ray, out enter))
            transform.position = ray.GetPoint(enter);
    }

    private void OnMouseUp()
    {
        if (TurnManager.singleton.playerTurn != idTeam)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sortingOrder = 10;

        Vector2Int pos = new Vector2Int();
        pos.x = Mathf.FloorToInt(transform.position.x);
        pos.y = Mathf.FloorToInt(transform.position.y);

        if (!haveMove && enableMovePosition.Contains(pos))
        {
            if (!haveMove)
            {
                transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
                position = pos;
                haveMove = true;
            }
            Map.singleton.DisplayMovableCells(false);
            enableAttackPosition = Map.singleton.GetAttackCells(this);
            if (enableAttackPosition.Count == 0)
            {
                haveAttack = true;
                GetComponent<SpriteRenderer>().color = Color.gray;
                TurnManager.singleton.CharacterEndPlay(this);
            }
        }
        else if(!haveAttack && enableAttackPosition.Contains(pos))
        {
            transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);

            int exp = Map.singleton.GetEntityAtPosition(pos).TakeDamage(attack, data.CharacterType);
            
            if (idTeam == 0)
            {
                AddExp(5 + exp);
            }

            haveAttack = true;
            haveMove = true;

            GetComponent<SpriteRenderer>().color = Color.gray;
            Map.singleton.DisplayMovableCells(false);
            Map.singleton.DisplayAttackableCells(false);
            TurnManager.singleton.CharacterEndPlay(this);
        }
        else
        {
            transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        }
    }

    public void ResetTurn()
    {
        haveMove = false;
        haveAttack = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    override public int TakeDamage(int _damage, CharacterType _type)
    {
        if ((_type == CharacterType.Red && data.CharacterType == CharacterType.Green) ||
            (_type == CharacterType.Green && data.CharacterType == CharacterType.Blue) ||
            (_type == CharacterType.Blue && data.CharacterType == CharacterType.Red))
        {
            _damage *= 2;
        }


        hp -= _damage;
        textHp.text = Mathf.Max(0, hp).ToString();
        imageLifeBar.fillAmount = hp * 1f / maxHp;
        if (hp <= 0)
        {
            TurnManager.singleton.RemoveCharacter(this);
            Map.singleton.RemoveEntity(this);
            // add anim
            Destroy(gameObject, 0.2f);
            return lvl;
        }
        return 0;
    }

    void AddExp(int _amout)
    {
        exp += _amout;
        while(exp >= CharacterProgression.GetExpNeededAtlvl(lvl))
        {
            exp -= CharacterProgression.GetExpNeededAtlvl(lvl);
            lvl++;
            SetData(data, lvl, exp);
            UIGame.singleton.DisplayCharacterUp(this);
        }
        UIGame.singleton.DisplayCharacterInfo(this);
        DataManager.singleton.UpdateProgression(progress.idCollection, lvl, exp);
    }
}
