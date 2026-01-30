using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject stoneBlock, dirtBlock, grassBlock, stones, player, coin;
    GameObject startBlock, endBlock;
    int n, k; 
    public int currentLevel;
    GameObject[] stoneBlocks;
    Camera cam;
    public Sprite[] selectedSprite = new Sprite[3];
    public Sprite[] defaultSprite = new Sprite[3];
    Block selectedBlock;
    BaseBlock[] path;
    int pathLength;
    bool canTouch;

    void Start()
    {
        loadGame();
        //deleteGame();
        FindObjectOfType<UIManager>().levelNumber.text = "Level " + (currentLevel + 1).ToString();
        if (currentLevel == 0) FindObjectOfType<UIManager>().tutorial.SetActive(true);

        selectedBlock = null;

        cam = FindObjectOfType<Camera>();
        k = 0; pathLength = 0; canTouch = true;
        System.Random ran = new System.Random();
        n = (currentLevel + 1) % 50 == 0 ? 10 : (currentLevel + 1) <= 20 ? ran.Next(3, 5) : (currentLevel + 1) <= 50 ? ran.Next(3, 6) : (currentLevel + 1) <= 100 ? ran.Next(3, 7) : (currentLevel + 1) <= 250 ? ran.Next(4, 8) : ran.Next(5, 9);
        cam.orthographicSize = n + 1;
        
        stoneBlocks = new GameObject[n * n];
        path = new BaseBlock[n * n];
        levelBuilder();
    }

    public void levelBuilder()
    {
        for (int i = n / 2; i > ((n % 2) + (n / 2)) * -1; i--)
        {
            for (int j = (n / 2) * -1; j < (n / 2) + (n % 2); j++)
            {
                GameObject g;
                if (n % 2 == 1)
                    g = Instantiate(stoneBlock, new Vector2(0.86f * j, 0.83f * i), Quaternion.identity);
                else
                    g = Instantiate(stoneBlock, new Vector2(0.86f * j + 0.43f, 0.83f * i - 0.415f), Quaternion.identity);
                stoneBlocks[k] = g;
                g.GetComponent<Block>().baseBlock = g.GetComponent<BaseBlock>();
                g.GetComponent<BaseBlock>().id = k++;
            }
        }

        for(int i = 0; i < n; i++)
        {
            for(int j = i * n; j < i * n + n; j++)
            {
                BaseBlock bb = stoneBlocks[j].GetComponent<BaseBlock>();
                if(j % n != 0)
                    bb.neighbors[bb.neighborsNumber++] = stoneBlocks[j - 1].GetComponent<BaseBlock>();
                if (j % n != n - 1)
                    bb.neighbors[bb.neighborsNumber++] = stoneBlocks[j + 1].GetComponent<BaseBlock>();
                if (i != 0)
                    bb.neighbors[bb.neighborsNumber++] = stoneBlocks[j - n].GetComponent<BaseBlock>();
                if (i != n - 1)
                    bb.neighbors[bb.neighborsNumber++] = stoneBlocks[j + n].GetComponent<BaseBlock>();
            }
        }

        System.Random ra = new System.Random();
        int blockIndexS = ra.Next(n);
        startBlock = Instantiate(stoneBlock, new Vector2(stoneBlocks[blockIndexS].transform.position.x, stoneBlocks[blockIndexS].transform.position.y + 0.83f), Quaternion.identity);
        startBlock.GetComponent<SpriteRenderer>().sortingOrder = -1;
        startBlock.GetComponent<BaseBlock>().neighbors[0] = stoneBlocks[blockIndexS].GetComponent<BaseBlock>();
        stoneBlocks[blockIndexS].GetComponent<BaseBlock>().neighbors[stoneBlocks[blockIndexS].GetComponent<BaseBlock>().neighborsNumber++] = startBlock.GetComponent<BaseBlock>();
        startBlock.GetComponent<BaseBlock>().neighborsNumber++;
        startBlock.GetComponent<Block>().baseBlock = startBlock.GetComponent<BaseBlock>();
        startBlock.GetComponent<BaseBlock>().id = -1;
        path[pathLength++] = startBlock.GetComponent<BaseBlock>();
        player = Instantiate(player, new Vector2(startBlock.transform.position.x - 0.04207f, startBlock.transform.position.y + 0.05238f), Quaternion.identity);
        player.GetComponent<SpriteRenderer>().sortingOrder = 101;

        System.Random ra2 = new System.Random();
        int blockIndexE = ra2.Next(n) + (n * (n - 1));
        endBlock = Instantiate(stoneBlock, new Vector2(stoneBlocks[blockIndexE].transform.position.x, stoneBlocks[blockIndexE].transform.position.y - 0.83f), Quaternion.identity);
        endBlock.GetComponent<BaseBlock>().neighbors[0] = stoneBlocks[blockIndexE].GetComponent<BaseBlock>();
        stoneBlocks[blockIndexE].GetComponent<BaseBlock>().neighbors[stoneBlocks[blockIndexE].GetComponent<BaseBlock>().neighborsNumber++] = endBlock.GetComponent<BaseBlock>();
        endBlock.GetComponent<BaseBlock>().neighborsNumber++;
        endBlock.GetComponent<Block>().baseBlock = endBlock.GetComponent<BaseBlock>();
        endBlock.GetComponent<BaseBlock>().id = -2;
        coin = Instantiate(coin, new Vector2(endBlock.transform.position.x - 0.04207f, endBlock.transform.position.y + 0.05238f), Quaternion.identity);
        coin.GetComponent<SpriteRenderer>().sortingOrder = 100;

        for (int i = 0; i < (n - 1) * (n - 1); i++)
        {
            System.Random random = new System.Random();
            int r = random.Next(k);
            GameObject g;
            if (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber == 0)
            {
                g = Instantiate(dirtBlock, new Vector2(stoneBlocks[r].transform.position.x - 0.04207f, stoneBlocks[r].transform.position.y + 0.05238f), Quaternion.identity);
                g.GetComponent<SpriteRenderer>().sortingOrder = r + 1;
                stoneBlocks[r].GetComponent<BaseBlock>().layers[stoneBlocks[r].GetComponent<BaseBlock>().layersNumber++] = g.GetComponent<Block>();
                g.GetComponent<Block>().baseBlock = stoneBlocks[r].GetComponent<BaseBlock>();
            }
            else i--;
        }

        for (int i = 0; i < (n * n) / 2; i++)
        {
            System.Random random = new System.Random();
            int r = random.Next(k);
            GameObject g;
            if (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber == 0 || 
                (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber == 1 && 
                stoneBlocks[r].GetComponent<BaseBlock>().layers[stoneBlocks[r].GetComponent<BaseBlock>().layersNumber - 1].order != 2))
            {
                g = Instantiate(grassBlock, 
                    new Vector2(stoneBlocks[r].transform.position.x - (0.04207f * (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber + 1)), 
                    stoneBlocks[r].transform.position.y + (0.05238f * (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber + 1))), Quaternion.identity);
                g.GetComponent<SpriteRenderer>().sortingOrder = r + 2;
                stoneBlocks[r].GetComponent<BaseBlock>().layers[stoneBlocks[r].GetComponent<BaseBlock>().layersNumber++] = g.GetComponent<Block>();
                g.GetComponent<Block>().baseBlock = stoneBlocks[r].GetComponent<BaseBlock>();
            }
            else i--;
        }

        for (int i = 0; i < (n * n) / 2; i++)
        {
            System.Random random = new System.Random();
            int r = random.Next(k);
            GameObject g;
            if (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber == 0 ||
                (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber >= 1 &&
                stoneBlocks[r].GetComponent<BaseBlock>().layers[stoneBlocks[r].GetComponent<BaseBlock>().layersNumber - 1].order != 3))
            {
                g = Instantiate(stones,
                    new Vector2(stoneBlocks[r].transform.position.x - (0.04207f * (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber + 1)),
                    stoneBlocks[r].transform.position.y + (0.05238f * (stoneBlocks[r].GetComponent<BaseBlock>().layersNumber + 1))), Quaternion.identity);
                g.GetComponent<SpriteRenderer>().sortingOrder = r + 3;
                stoneBlocks[r].GetComponent<BaseBlock>().layers[stoneBlocks[r].GetComponent<BaseBlock>().layersNumber++] = g.GetComponent<Block>();
                g.GetComponent<Block>().baseBlock = stoneBlocks[r].GetComponent<BaseBlock>();
            }
            else i--;
        }
    }

    private IEnumerator pathFinder(BaseBlock bb)
    {
        if (bb.id == -2)
        {
            canTouch = false;
            
            yield return StartCoroutine(walkAlongPath(path, pathLength));
            currentLevel++;
            saveGame();
            yield return null;
            FindObjectOfType<UIManager>().restart();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else
        {
            bb.isVisited = true;
            for(int i = 0; i < bb.neighborsNumber; i++) {
                BaseBlock b = bb.neighbors[i];
                if(!b.isVisited && b.layersNumber == 0)
                {
                    path[pathLength++] = b;
                    yield return StartCoroutine(pathFinder(b));
                    path[pathLength - 1] = null;
                    pathLength--;
                }
            }
            bb.isVisited = false;
        }
    }

    private IEnumerator walkAlongPath (BaseBlock[] p, int size)
    {
        for (int i = 1; i < size; i++)
        {
            float t = 0;
            BaseBlock block = p[i];
            Vector2 StartPos = player.transform.position;
            Vector2 EndPos = new Vector2(block.transform.position.x - 0.04207f, block.transform.position.y + 0.05238f);
            if (player.transform.position.x > EndPos.x)
                player.transform.rotation = new Quaternion(0, 0, -90, 90);
            else if (player.transform.position.x < EndPos.x)
                player.transform.rotation = new Quaternion(0, 0, 90, 90);
            else if (player.transform.position.y > EndPos.y)
                player.transform.rotation = new Quaternion(0, 0, 0, 0);
            else if (player.transform.position.y < EndPos.y)
                player.transform.rotation = new Quaternion(0, 0, 180, 90);

            while (t < 1)
            {
                player.transform.position = Vector2.Lerp(StartPos, EndPos, t);
                t += Time.deltaTime * 3;
                yield return null;
            }

            player.transform.position = EndPos;
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && canTouch)
        {
            Vector2 touchPosition = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
            if(hit.collider != null)
            {
                if(selectedBlock == null)
                {
                    selectedBlock = hit.collider.GetComponent<Block>();
                    if (selectedBlock.baseBlock.layersNumber != 0)
                    {
                        selectedBlock = selectedBlock.baseBlock.layers[selectedBlock.baseBlock.layersNumber - 1];
                        selectedBlock.gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite[selectedBlock.order - 1];
                    }
                    else
                        selectedBlock = null;
                }
                else
                {
                    Block targetBlock = hit.collider.GetComponent<Block>();
                    if(targetBlock.baseBlock.id >= 0)
                    {
                        if (targetBlock.baseBlock.layersNumber == 0)
                        {
                            selectedBlock.gameObject.transform.position = new Vector2(targetBlock.transform.position.x - 0.04207f, targetBlock.transform.position.y + 0.05238f);
                            targetBlock.gameObject.GetComponent<BaseBlock>().layers[targetBlock.gameObject.GetComponent<BaseBlock>().layersNumber++] = selectedBlock;
                            selectedBlock.baseBlock.layers[selectedBlock.baseBlock.layersNumber - 1] = null;
                            selectedBlock.baseBlock.layersNumber--;
                            selectedBlock.gameObject.GetComponent<SpriteRenderer>().sortingOrder = targetBlock.baseBlock.id + 1;
                            selectedBlock.baseBlock = targetBlock.gameObject.GetComponent<BaseBlock>();
                            selectedBlock.gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite[selectedBlock.order - 1];
                            selectedBlock = null;
                            StartCoroutine(pathFinder(startBlock.GetComponent<BaseBlock>())); 
                        }
                        else
                        {
                            targetBlock = targetBlock.baseBlock.layers[targetBlock.baseBlock.layersNumber - 1];
                            if (targetBlock.order < selectedBlock.order)
                            {
                                selectedBlock.gameObject.transform.position = new Vector2(targetBlock.transform.position.x - 0.04207f, targetBlock.transform.position.y + 0.05238f);
                                targetBlock.baseBlock.layers[targetBlock.baseBlock.layersNumber++] = selectedBlock;
                                selectedBlock.baseBlock.layers[selectedBlock.baseBlock.layersNumber - 1] = null;
                                selectedBlock.baseBlock.layersNumber--;
                                selectedBlock.gameObject.GetComponent<SpriteRenderer>().sortingOrder = targetBlock.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
                                selectedBlock.baseBlock = targetBlock.baseBlock;
                                selectedBlock.gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite[selectedBlock.order - 1];
                                selectedBlock = null;
                                StartCoroutine(pathFinder(startBlock.GetComponent<BaseBlock>()));
                            }
                            else
                            {
                                selectedBlock.gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite[selectedBlock.order - 1];
                                targetBlock.gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite[targetBlock.order - 1];
                                selectedBlock = targetBlock;
                            }
                        }
                    }
                    else
                    {
                        if (selectedBlock != null)
                        {
                            selectedBlock.gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite[selectedBlock.order - 1];
                            selectedBlock = null;
                        }
                    }
                }
            }
            else
            {
                if (selectedBlock != null)
                {
                    selectedBlock.gameObject.GetComponent<SpriteRenderer>().sprite = defaultSprite[selectedBlock.order - 1];
                    selectedBlock = null;
                }
            }
        }
    }

    [System.Serializable]
    class SaveData
    {
        public int currentLevel;
    }

    string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public void saveGame()
    {
        SaveData sd = new SaveData();
        sd.currentLevel = currentLevel;
        string json = JsonUtility.ToJson(sd);
        File.WriteAllText(savePath, json);
    }

    public void loadGame()
    {
        if(File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData sd = JsonUtility.FromJson<SaveData>(json);
            currentLevel = sd.currentLevel;
        }
    }

    public void deleteGame()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}
