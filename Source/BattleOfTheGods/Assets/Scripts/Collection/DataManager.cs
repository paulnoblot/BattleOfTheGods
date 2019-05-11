using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CharacterProgression
{
    public int idCollection;
    public int idCharacter;
    public int lvl;
    public int exp;

    public CharacterProgression(int _idCollection, int _idCharacter, int _lvl = 1, int _exp = 0)
    {
        idCollection = _idCollection;
        idCharacter = _idCharacter;
        lvl = _lvl;
        exp = _exp;
    }

    public static int GetExpNeededAtlvl(int _lvl)
    {
        return 10 + 2 * _lvl;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager singleton;

    List<CharacterData> database;
    Dictionary<int, CharacterProgression> playerCollection;
    int[] team;
    int idColl = 1;
    public int stageLevel = 1;

    string pathTeam = Path.Combine(Application.streamingAssetsPath, "Save/Team.txt");
    string pathCollection = Path.Combine(Application.streamingAssetsPath, "Save/Collection.txt");

    string keyTeam = "team";
    string keyCollection = "collection";

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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey(keyTeam) || !PlayerPrefs.HasKey(keyCollection))
        {
            ResetPlayerPrefs();
        }

        LoadCharacter();
        LoadPlayerCollection();
        LoadTeam();
    }

    void LoadCharacter()
    {
        database = new List<CharacterData>();
        Object[] objsChar = Resources.LoadAll("Characters", typeof(CharacterData));
        foreach (var item in objsChar)
        {
            database.Add((CharacterData)item);
        }
    }

    void LoadPlayerCollection()
    {
        playerCollection = new Dictionary<int, CharacterProgression>();
        
        string[] splittedline;
        CharacterProgression progression;


        string[] strColl = PlayerPrefs.GetString(keyCollection).Split('\n');

        idColl = 1;

        for (int i = 0; i < strColl.Length; i++)
        {
            splittedline = strColl[i].Split(',');
            progression = new CharacterProgression(idColl, int.Parse(splittedline[0]), int.Parse(splittedline[1]), int.Parse(splittedline[2]));
            playerCollection.Add(idColl, progression);
            idColl++;
        }

        // StreamReader reader = new StreamReader(pathCollection);
        // while ((line = reader.ReadLine()) != null)
        // {
        //     splittedline = line.Split(',');
        //     progression = new CharacterProgression(idColl, int.Parse(splittedline[0]), int.Parse(splittedline[1]), int.Parse(splittedline[2]));
        //     playerCollection.Add(idColl, progression);
        //     idColl++;
        // }
        // reader.Close();
    }

    void LoadTeam()
    {

        team = new int[4];

        string[] strTeam = PlayerPrefs.GetString(keyTeam).Split('\n');

        for (int i = 0; i < 4; i++)
        {
            team[i] = int.Parse(strTeam[i]);
        }

        // StreamReader reader = new StreamReader(pathTeam);
        // team[0] = int.Parse(reader.ReadLine());
        // team[1] = int.Parse(reader.ReadLine());
        // team[2] = int.Parse(reader.ReadLine());
        // team[3] = int.Parse(reader.ReadLine());
        // reader.Close();
    }

    public void SaveTeam(int[] _idCharacters)
    {
        team = _idCharacters;
        
        string teamStr = team[0] + "\n" + team[1] + "\n" + team[2] + "\n" + team[3];
        PlayerPrefs.SetString(keyTeam, teamStr);

        // StreamWriter writer = new StreamWriter(pathTeam, false);
        // writer.WriteLine(_idCharacters[0]);
        // writer.WriteLine(_idCharacters[1]);
        // writer.WriteLine(_idCharacters[2]);
        // writer.WriteLine(_idCharacters[3]);
        // writer.Close();
    }
    
    public void AddInCollection(int _idCharacter, int _lvl = 1)
    {
        playerCollection.Add(idColl, new CharacterProgression(idColl, _idCharacter));
        idColl++;
        SaveCollection();
    }

    public List<int> GetNonPossededCharacter()
    {
        List<int> toReturn = new List<int>();
        foreach (CharacterData data in database)
            if (!playerCollection.ContainsKey(data.Id))
                toReturn.Add(data.Id);
        return toReturn;
    }

    public void SaveCollection()
    {
        List<CharacterProgression> toSave = GetCollection();
        string collStr = toSave[0].idCharacter + "," + toSave[0].lvl + "," + toSave[0].exp;
        for (int i = 1; i < toSave.Count; i++)
        {
            collStr += "\n" + toSave[i].idCharacter + "," + toSave[i].lvl + "," + toSave[i].exp;
        }

        PlayerPrefs.SetString(keyCollection, collStr);
        LoadPlayerCollection();

        // StreamWriter writer = new StreamWriter(pathCollection, false);
        // foreach (KeyValuePair<int, CharacterProgression> item in playerCollection)
        // {
        //     writer.WriteLine(item.Value.idCharacter + "," + item.Value.lvl + "," + item.Value.exp);
        // }        // writer.Close();
    }

    public List<CharacterData> GetDatabase()
    {
        return database;
    }

    public List<CharacterProgression> GetCollection()
    {
        return playerCollection.Select(x => x.Value).ToList();
    }

    public CharacterData GetData(int _id)
    {
        foreach (CharacterData data in database)
            if (data.Id == _id)
                return data;
        return null;
    }

    public void UpdateProgression(int _idColl, int _lvl, int _exp)
    {
        playerCollection[_idColl].lvl = _lvl;
        playerCollection[_idColl].exp = _exp;
        SaveCollection();
    }

    public CharacterProgression GetProgression(int _idColl)
    {
        return playerCollection.ContainsKey(_idColl) ? playerCollection[_idColl] : null;
    }

    public CharacterData GetRandomData()
    {
        return database[Random.Range(0,database.Count)];
    }

    public int[] GetTeam()
    {
        return team;
    }

    public void ResetPlayerPrefs()
    {
        string defaultTeam = "1\n2\n3\n4";
        PlayerPrefs.SetString(keyTeam, defaultTeam);
        string defaultCollection = "1,1,0\n2,1,0\n3,1,0\n4,1,0\n5,1,0";
        PlayerPrefs.SetString(keyCollection, defaultCollection);
        Start();
    }
}
