using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

public enum CharacterType
{
    None,
    Red,
    Blue,
    Green
}

[CreateAssetMenu(menuName = "CharacterData", fileName = "newCharacter")]
public class CharacterData : ScriptableObject
{
    [SerializeField] int id;

    // stats
    [Header("Base Stats")]
    [SerializeField] int hp;
    [SerializeField] int attack;
    [SerializeField] int movement;
    [SerializeField] int attackRange;
    [SerializeField] CharacterType characterType;

    // progression
    [Header("Progression")]
    [SerializeField] float hpPerLvl;
    [SerializeField] float attackPerLvl;

    [Header("View")]
    [SerializeField] Sprite picture;
    [SerializeField] GameObject model;

    public int Id { get { return id; } }

    public int Hp { get { return hp; } }
    public int Attack { get { return attack; } }
    public int Movement { get { return movement; } }
    public int AttackRange { get { return attackRange; } }
    public CharacterType CharacterType { get { return characterType; } }

    public float HpPerLvl { get { return hpPerLvl; } }
    public float AttackPerLvl { get { return attackPerLvl; } }

   public Sprite Picture { get { return picture; } }
   public GameObject Model { get { return model; } }

    public int GetHpAtLvl(int _lvl)
    {
        return hp + (int)((_lvl - 1) * hpPerLvl);
    }

    public int GetAttackAtLvl(int _lvl)
    {
        return attack + (int)((_lvl - 1) * attackPerLvl);
    }
}

