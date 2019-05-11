using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int idTeam;
    public Vector2Int position;

    public virtual int TakeDamage(int _damageAmout, CharacterType _type)
    {
        return 0;
    }
}
