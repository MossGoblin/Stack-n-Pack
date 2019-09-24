using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageAreaCreator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SpriteTileMode floorTile;
    [SerializeField] public int storageAreaW;
    [SerializeField] public int storageAreaH;
    [SerializeField] GameObject concreteTile;
    [SerializeField] Transform tilesParent;
    [SerializeField] Transform player;
    [SerializeField] Transform crateMaster;
    public int storageAreaOriginW;
    public int storageAreaOriginH;
    public int storageAreaEndPointW;
    public int storageAreaEndPointH;

    // shaded tile sprites
    [SerializeField] Sprite defaultTileSprite;
    [SerializeField] Sprite groupTileSprite0;
    [SerializeField] Sprite groupTileSprite1;
    [SerializeField] Sprite groupTileSprite2;
    [SerializeField] Sprite groupTileSprite3;
    [SerializeField] Sprite groupTileSprite4;
    [SerializeField] Sprite groupTileSprite5;

    private bool[,] vacancyGrid;
    CrateMaster crateController;

    void Awake()
    {
        // init vacancy grid
        vacancyGrid = new bool[storageAreaW, storageAreaH];

        for (int countWidth = 0; countWidth < storageAreaW; countWidth++)
        {
            for (int countHeight = 0; countHeight < storageAreaH; countHeight++)
            {
                MarkVacancyGrid(countWidth, countHeight, true);
            }
        }
    }
    // Update is called once per frame
    private void Update()
    {
        RecolorGrid();
        // TODO :: HERE
    }

    private void RecolorGrid()
    {
        CrateMaster crateController = crateMaster.GetComponent<CrateMaster>();
        for (int countY = 0; countY < storageAreaH; countY++)
        {
            for (int countX = 0; countX < storageAreaW; countX++)
            {
                    SpriteRenderer tileSpriteRenderer = FindTileAt(countX, countY).GetComponent<SpriteRenderer>();
                    switch(crateController.groupGrid[countX, countY])
                    {
                        case 0:
                            tileSpriteRenderer.sprite = defaultTileSprite;
                            break;
                        case 1:
                            tileSpriteRenderer.sprite = groupTileSprite1;
                            break;
                        case 2:
                            tileSpriteRenderer.sprite = groupTileSprite2;
                            break;
                        case 3:
                            tileSpriteRenderer.sprite = groupTileSprite3;
                            break;
                        case 4:
                            tileSpriteRenderer.sprite = groupTileSprite4;
                            break;
                        case 5:
                            tileSpriteRenderer.sprite = groupTileSprite5;
                            break;
                        default:
                            tileSpriteRenderer.sprite = groupTileSprite0;
                            break;
                    }

            }
        }
    }

    private GameObject FindTileAt(int countX, int countY)
    {
        int coordX = countX + storageAreaOriginW;
        int coordY = countY + storageAreaOriginH;
        GameObject tile = GameObject.Find("tile " + coordX + " / " + coordY);
        if (tile != null)
        {
            return tile;
        }
        return null;
    }

    internal bool HasSpace()
    {
        int numberOfSpaces = 0;
        for (int countX = 0; countX < storageAreaW; countX++)
        {
            for (int countY = 0; countY < storageAreaH; countY++)
            {
                if (vacancyGrid[countX, countY])
                {
                    numberOfSpaces++;
                }
            }
        }
        if (numberOfSpaces >= 2)
        {
            return true;
        }
        Debug.Log("Out Of Space!");
        return false;
    }
    public bool CreateFloor()
    {
        storageAreaOriginW = (storageAreaW / 2) * -1;
        storageAreaOriginH = (storageAreaH / 2) * -1;
        storageAreaEndPointW = storageAreaW + storageAreaOriginW - 1;
        storageAreaEndPointH = storageAreaH + storageAreaOriginH - 1;

        for (int countH = 0; countH < storageAreaH; countH++)
        {
            for (int countW = 0; countW < storageAreaW; countW++)
            {
                float positionX = countW + storageAreaOriginW;
                float positionY = countH + storageAreaOriginH;
                GameObject newTile = Instantiate(concreteTile, new Vector3(positionX, positionY), Quaternion.identity);
                newTile.transform.SetParent(tilesParent);
                newTile.name = "tile " + positionX + " / " + positionY;
                // Debug.Log("placed a tile at: " + countX + " / " +  countY);
            }
        }
        return true;
    }

    public bool IsTileAvailableForCrate(int coordW, int coordH)
    {
        int playerWidth = (int)(player.transform.position.x - storageAreaOriginW);
        int playerHeight = (int)(player.transform.position.y - storageAreaOriginH);
        if ((coordW != playerWidth) || (coordH != playerHeight))
        {
            bool result = vacancyGrid[coordW, coordH];
            return vacancyGrid[coordW, coordH];
        }
        else
        {
            return false;
        }
        //Debug.Log("[" + coordW + " / " + coordH + "] : " + vacancyGrid[coordW, coordH]);
    }
    public bool IsTileVacant(int coordX, int coordY)
    {
        //return vacancyGrid[coordX + storageAreaOriginW, coordY + storageAreaOriginH];
        return vacancyGrid[coordX, coordY];
    }

    public void MarkVacancyGrid(int coordW, int coordH, bool free)
    {
        vacancyGrid[coordW, coordH] = free;
    }

    public int GetAbsFromRelW(int relW)
    {
        return relW - storageAreaOriginW;
    }

    public int GetAbsFromRelH(int relH)
    {
        return relH - storageAreaOriginH;
    }

    //public int GetAbsFromDeltaW(int x, int currentW)
    //{
    //    return currentW + x - storageAreaOriginW;
    //}

    //public int GetAbsFromDeltaH(int y, int currentH)
    //{
    //    return y + currentH - storageAreaOriginH;
    //}
}
