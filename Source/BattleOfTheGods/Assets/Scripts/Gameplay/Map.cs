using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public static Map singleton;

    [SerializeField] GameObject characterModel = null;
    [SerializeField] int nbMap = 1;
    [SerializeField] GameObject cam = null;
    int seed = 0;

    Tilemap tileMap;
    Tilemap infoMap;
    int[,] terrain;
    List<Vector2Int> allySpawns;
    List<Vector2Int> enemySpawns;

    // Pathfinding
    List<Entity> entities;
    Entity currentEntity;
    List<GameObject> movableCells = new List<GameObject>();
    List<GameObject> attackableCells = new List<GameObject>();
    public GameObject blue = null;
    public GameObject red = null;
    List<SearchNode> openNodes;
    List<Vector2Int> closedNodes;
    SearchNode enemyTarget;

    private void Awake()
    {
        if (singleton != null && singleton != this)
            Destroy(this);
        else
            singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        tileMap = transform.GetChild(0).GetComponent<Tilemap>();
        infoMap = transform.GetChild(1).GetComponent<Tilemap>();
        entities = new List<Entity>();
        allySpawns = new List<Vector2Int>();
        enemySpawns = new List<Vector2Int>();

        terrain = new int[6, 8];

        seed = Random.Range(0, nbMap);
        seed = 0;

        cam.transform.position = new Vector3(seed * 6 + 3, 4, -10);

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (infoMap.GetTile(new Vector3Int(seed * 6 + i, j, 0)))
                {
                    if (infoMap.GetTile(new Vector3Int(seed * 6 + i, j, 0)).name == "collision")
                        terrain[i, j] = 1;
                    else
                        terrain[i, j] = 0;

                    if (infoMap.GetTile(new Vector3Int(seed * 6 + i, j, 0)).name == "allySpawnPoint")
                        allySpawns.Add(new Vector2Int(seed * 6 + i, j));
                    else if (infoMap.GetTile(new Vector3Int(seed * 6 + i, j, 0)).name == "enemySpawnPoint")
                        enemySpawns.Add(new Vector2Int(seed * 6 + i, j));
                }
            }
        }

        transform.GetChild(1).GetComponent<TilemapRenderer>().enabled = false;

        int[] allyTeam = DataManager.singleton.GetTeam();

        for (int i = 0; i < allyTeam.Length; i++) // instantiate ally
        {
            if (allyTeam[i] != 0)
            {
                CharacterProgression progress = DataManager.singleton.GetProgression(allyTeam[i]);
                CharacterData data = DataManager.singleton.GetData(progress.idCharacter);
                GameObject newCharacter = Instantiate(characterModel, new Vector3(allySpawns[i].x + 0.5f, allySpawns[i].y + 0.5f, 0), Quaternion.identity);
                newCharacter.GetComponent<Character>().SetData(data, progress);
                newCharacter.GetComponent<Character>().position = allySpawns[i];
                newCharacter.GetComponent<Character>().SetTeam(0);
                entities.Add(newCharacter.GetComponent<Character>());
            }
        }
        
        for (int i = 0; i < 4; i++) // instantiate enemy
        {
            CharacterData data = DataManager.singleton.GetRandomData();
            GameObject newCharacter = Instantiate(characterModel, new Vector3(enemySpawns[i].x + 0.5f, enemySpawns[i].y + 0.5f, 0), Quaternion.identity);
            newCharacter.GetComponent<Character>().SetData(data, DataManager.singleton.stageLevel);
            newCharacter.transform.localScale = new Vector3(-1, 1, 1);
            newCharacter.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
            newCharacter.GetComponent<Character>().position = enemySpawns[i];
            newCharacter.GetComponent<Character>().SetTeam(1);
            entities.Add(newCharacter.GetComponent<Character>());
        }

        // entities = FindObjectsOfType<Entity>().ToList();
    }
    

    public List<Vector2Int> GetMoveCells(Character _character)
    {
        currentEntity = _character;

        openNodes = new List<SearchNode>();
        closedNodes = new List<Vector2Int>();
        openNodes.Add(new SearchNode(_character.position, _character.data.Movement));
        SearchMoveCells();

        for (int i = closedNodes.Count() - 1; i >= 0; i--) // remove ally cells
        {
            if (entities.Where(x => x.position == closedNodes[i]).Count() != 0)
            {
                closedNodes.RemoveAt(i);
            }
        }

        DisplayMovableCells(true);

        currentEntity = null;

        return closedNodes;
    }
    
    void SearchMoveCells()
    {
        SearchNode currentNode;

        while (openNodes.Count != 0)
        {
            currentNode = openNodes[0];

            if (currentNode.mvtPoint > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    SearchNode nextNode = new SearchNode(currentNode.position + neighbours[i], currentNode.mvtPoint - 1);
                    if (openNodes.Where(x => x.position == nextNode.position).Count() == 0 && 
                        !closedNodes.Contains(nextNode.position) &&
                        nextNode.position.x >= 0 && nextNode.position.x < terrain.GetLength(0) &&
                        nextNode.position.y >= 0 && nextNode.position.y < terrain.GetLength(1) &&
                        terrain[nextNode.position.x, nextNode.position.y] == 0 &&
                        entities.Where(x => x.position == nextNode.position && x.idTeam != currentEntity.idTeam).Count() == 0)
                    {
                        openNodes.Add(nextNode);
                    }
                }
            }

            closedNodes.Add(currentNode.position);
            openNodes.RemoveAt(0);
            openNodes = openNodes.OrderBy(x => x.mvtPoint).ToList();
        }
    }
    
    public List<Vector2Int> GetAttackCells(Character _character)
    {
        currentEntity = _character;

        openNodes = new List<SearchNode>();
        closedNodes = new List<Vector2Int>();
        openNodes.Add(new SearchNode(_character.position, _character.data.AttackRange));
        SearchAttackRangeCells();

        for (int i = closedNodes.Count() - 1; i >= 0; i--) // remove ally cells
        {
            if (entities.Where(x => x.position == closedNodes[i] && x.idTeam != _character.idTeam).Count() == 0)
            {
                closedNodes.RemoveAt(i);
            }
        }

        DisplayAttackableCells(true);

        currentEntity = null;

        return closedNodes;
    }

    void SearchAttackRangeCells()
    {
        SearchNode currentNode;

        while (openNodes.Count != 0)
        {
            currentNode = openNodes[0];

            if (currentNode.mvtPoint > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    SearchNode nextNode = new SearchNode(currentNode.position + neighbours[i], currentNode.mvtPoint - 1);
                    if (openNodes.Where(x => x.position == nextNode.position).Count() == 0 &&
                        !closedNodes.Contains(nextNode.position) &&
                        nextNode.position.x >= 0 && nextNode.position.x < terrain.GetLength(0) &&
                        nextNode.position.y >= 0 && nextNode.position.y < terrain.GetLength(1) // &&
                        // terrain[nextNode.position.x, nextNode.position.y] == 0 // can attack throw water
                        )
                    {
                        openNodes.Add(nextNode);
                    }
                }
            }

            closedNodes.Add(currentNode.position);
            openNodes.RemoveAt(0);
            openNodes = openNodes.OrderBy(x => x.mvtPoint).ToList();
        }
    }

    void SearchNearsetEnemy()
    {
        SearchNode currentNode;

        while (openNodes.Count != 0)
        {
            currentNode = openNodes[0];

            for (int i = 0; i < 4; i++)
            {
                SearchNode nextNode = new SearchNode(currentNode.position + neighbours[i], currentNode.mvtPoint + 1);
                if (openNodes.Where(x => x.position == nextNode.position).Count() == 0 &&
                    !closedNodes.Contains(nextNode.position) &&
                    nextNode.position.x >= 0 && nextNode.position.x < terrain.GetLength(0) &&
                    nextNode.position.y >= 0 && nextNode.position.y < terrain.GetLength(1))
                {
                    nextNode.parentNode = currentNode;
                    openNodes.Add(nextNode);
                    if (entities.Where(x => x.idTeam == 0 && x.position == nextNode.position).Count() > 0)
                    {
                        enemyTarget = nextNode;
                        return;
                    }
                }
            }

            closedNodes.Add(currentNode.position);
            openNodes.RemoveAt(0);
            openNodes = openNodes.OrderByDescending(x => x.mvtPoint).ToList();
        }
    }

    

    public void DisplayMovableCells(bool display)
    {
        foreach (var item in movableCells)
            Destroy(item);
        movableCells.Clear();

        if(display)
        foreach (var item in closedNodes)
        {
            movableCells.Add(Instantiate(blue, new Vector3(item.x + 0.5f, item.y + 0.5f, 0), Quaternion.identity, transform));
        }
    }

    public void DisplayAttackableCells(bool display)
    {
        foreach (var item in attackableCells)
            Destroy(item);
        attackableCells.Clear();

        if (display)
            foreach (var item in closedNodes)
            {
                attackableCells.Add(Instantiate(red, new Vector3(item.x + 0.5f, item.y + 0.5f, 0), Quaternion.identity, transform));
            }
    }

    Vector2Int[] neighbours = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };

    class SearchNode
    {
        public SearchNode parentNode;
        public Vector2Int position;
        public int mvtPoint;

        public SearchNode(Vector2Int _position, int _movementPoint)
        {
            position = _position;
            mvtPoint = _movementPoint;
        }
    }

    public Entity GetEntityAtPosition(Vector2Int _pos)
    {
        foreach (var entity in entities)
            if (entity.position == _pos)
                return entity;

        return null;
    }

    public void RemoveEntity(Entity _entity)
    {
        entities.Remove(_entity);
    }
}


