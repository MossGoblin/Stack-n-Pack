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

    // group coloring
    int[] baseRedSteps = new int[] { 0, -1, 0, 0, 1, 0 };
    float hueChangeFactor = 2f; // 8 by default; value of 1 would make the step duration 255 ticks
    [SerializeField] GameObject basePaletteTile;
    public Dictionary<int, int> colorChunks;
    public Color[] paletteArray;
    [SerializeField] float alpha = 0.5f; // alpha goes from 0f to 1f;
    // temp paletteTile container
    [SerializeField] GameObject paletteAnchor;
    // mapping for groups and color indeces
    public Dictionary<int, int> groupToColorMap;

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

        colorChunks = new Dictionary<int, int>();
        int paletteLength = (256 * 6 / (int)hueChangeFactor);
        paletteArray = new Color[paletteLength];
        groupToColorMap = new Dictionary<int, int>();

        BuildColorPalette();
    }
    // Update is called once per frame
    private void Update()
    {
        RecolorGrid();
    }

    private void RecolorGrid()
    {
        CrateMaster crateController = crateMaster.GetComponent<CrateMaster>();
        for (int countY = 0; countY < storageAreaH; countY++)
        {
            for (int countX = 0; countX < storageAreaW; countX++)
            {
                SpriteRenderer tileSpriteRenderer = FindTileAt(countX, countY).GetComponent<SpriteRenderer>();
                // find if the group exists in the groupToColor map - either assign its color or create a new color
                Color newColor;

                int tileGroup = crateController.groupGrid[countX, countY];

                // FIRST find if there is no group

                if (tileGroup == 0)
                {
                    newColor = new Color(1f, 1f, 1f, 1f);
                }
                else if (groupToColorMap.ContainsKey(tileGroup)) // the group has a mapped color
                {
                    newColor = paletteArray[groupToColorMap[tileGroup]];
                }
                else
                {
                    // create new color
                    int newColorIndex = SetUpNewColor()
;                   newColor = paletteArray[newColorIndex];
                    groupToColorMap.Add(tileGroup, newColorIndex);
                }
                tileSpriteRenderer.color = newColor;
            }
        }
    }

    private int SetUpNewColor() // add new color in colorChunks
    {
        // plan
        // find the largest chunk in colorChunks - LCC
        int largestChunkKey = 0;
        int largestChunkValue = 0;
        foreach (var item in colorChunks)
        {
            if (item.Value > largestChunkValue)
            {
                largestChunkKey = item.Key;
                largestChunkValue = item.Value;
            }
        }

        // create new color Chunk - <LCC Key + (LCC value-1)/2, LCC/2>; adjust value of LCC to (LCC-1)/2
        int newKey = (largestChunkKey + ((largestChunkValue - 1) / 2)) % (paletteArray.Length);
        int newValue = largestChunkValue / 2 - 1;
        colorChunks[largestChunkKey] = (largestChunkValue / 2);
        colorChunks.Add(newKey, newValue);
        // assign group number to new Key
        return newKey;
    }

    private bool RemoveColor() // remove a color from colorChunks
    {
        return true;
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

    public bool IsWithinBorders(int posX, int posY)
    {
        if ((posX >= 0 && posX < storageAreaW)
            && (posY >= 0 && posY < storageAreaH))
        {
            return true;
        }
        return false;
    }

    private bool BuildColorPalette()
    {

        // double cycle - the outer loop marks steps; the inner loop is for the duration of the steps

        // set base valued for colors
        float clrRed = 256;
        float clrGrn = 0;
        float clrBlu = 0;
        int paletteCounter = 0;
        float maxStepDuration = (256f / hueChangeFactor) - 1;

        for (float stepCounter = 0; stepCounter < 6; stepCounter += 1f) // outer loop 
        {
            for (float stepDuration = 0; stepDuration < maxStepDuration; stepDuration += 1) // inner loop
            {
                // fill in a palette grid
                float nrmRed = NormalizeColor(clrRed);
                float nrmGrn = NormalizeColor(clrGrn);
                float nrmBlu = NormalizeColor(clrBlu);
                Color newColor = new Color(nrmRed, nrmGrn, nrmBlu, 1f);
    
                PlacePaletteTile(paletteCounter, newColor);

                // fill in the color in the palette
                paletteArray[paletteCounter] = newColor;


                // update colors according to steps and duration
                clrRed += ((GetStep(0, (int)stepCounter)) * hueChangeFactor);
                clrGrn += ((GetStep(4, (int)stepCounter)) * hueChangeFactor);
                clrBlu += ((GetStep(2, (int)stepCounter)) * hueChangeFactor);


                paletteCounter++;
            }
        }
        // initiate first large groupStrip - pick a random point in the color array
        int randomStartingColorIndex = UnityEngine.Random.Range(0, paletteArray.Length - 1);
        colorChunks.Add(randomStartingColorIndex, paletteArray.Length-1);
        // map to the first possible group
        groupToColorMap[1] = randomStartingColorIndex;
        return true;
    }

    private int GetStep(int baseColor, int stepNumber) // for baseColor supply 0 for Red, 4 for Green, 2 for Blue
    {
        int newStep = baseRedSteps[(stepNumber + baseColor) % 6];
        return newStep;
    }

    private float NormalizeColor(float color)
    {

        float newValue = color / 255f;
        return newValue;
    }

    private bool PlacePaletteTile(float deltaX, Color color)
    {
        // TODO :: TBD PALETTE TILES
        // get the palette anchor coordinates
        float anchorX = GameObject.Find("PaletteAnchor").GetComponent<Transform>().position.x;
        float anchorY = GameObject.Find("PaletteAnchor").GetComponent<Transform>().position.y;

        float fullDeltaX = deltaX / 100f;
        GameObject newPaletteTile = Instantiate(basePaletteTile, new Vector3(anchorX + (fullDeltaX), anchorY), Quaternion.identity);
        newPaletteTile.GetComponent<SpriteRenderer>().color = color;
        newPaletteTile.GetComponent<Transform>().SetParent(paletteAnchor.GetComponent<Transform>());
        return true;
    }

}
