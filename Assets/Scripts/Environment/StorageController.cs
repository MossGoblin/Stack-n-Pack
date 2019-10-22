using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class StorageController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SpriteTileMode floorTile;
    [SerializeField] public int storageWidth;
    [SerializeField] public int storageHight;
    [SerializeField] GameObject concreteTile;
    [SerializeField] Transform tilesParent;
    [SerializeField] Transform player;
    [SerializeField] Transform crateMasterTransform;
    public int storageAreaOriginW;
    public int storageAreaOriginH;
    public int storageAreaEndPointW;
    public int storageAreaEndPointH;

    // shaded tile sprites
    [SerializeField] Sprite serviceTile;

    CrateController crateMaster;

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

    // factorywork
    [SerializeField] GameObject factoryPrefab;
    public Factory[] factoryList;

    // minimum size of grid - TEMP
    int storageMinW = 7;
    int storageMinH = 5;

    void Awake()
    {
        // build refs
        crateMaster = GameObject.FindObjectOfType<CrateController>();

        factoryList = new Factory[4];
        for (int count = 0; count < 4; count++)
        {
            factoryList[count] = null;
        }

        // TEMP - area size
        storageWidth = Mathf.Max(storageWidth, storageMinW);
        storageHight = Mathf.Max(storageHight, storageMinH);

        storageAreaOriginW = (storageWidth / 2) * -1;
        storageAreaOriginH = (storageHight / 2) * -1;
        storageAreaEndPointW = storageWidth + storageAreaOriginW - 1;
        storageAreaEndPointH = storageHight + storageAreaOriginH - 1;

        // init vacancy grid
        colorChunks = new Dictionary<int, int>();
        int paletteLength = (256 * 6 / (int)hueChangeFactor);
        paletteArray = new Color[paletteLength];
        groupToColorMap = new Dictionary<int, int>();

        BuildColorPalette();
    }

    private void Start()
    {
        // build refs
        crateMaster = FindObjectOfType<CrateController>();
        crateMasterTransform = crateMaster.GetComponent<Transform>();

        PlaceFactories();
    }

    // Update is called once per frame
    private void Update()
    {
        // highlight groups according to tiles groups
        RecolorGrid();
    }

    /// <summary>
    /// Initialize the content of the four factories
    /// </summary>

    private void RecolorGrid()
    {
        CrateController crateController = crateMasterTransform.GetComponent<CrateController>();
        for (int countY = 0; countY < storageHight; countY++)
        {
            for (int countX = 0; countX < storageWidth; countX++)
            {
                SpriteRenderer tileSpriteRenderer = FindTileAt(countX, countY).GetComponent<SpriteRenderer>();
                // find if the group exists in the groupToColor map - either assign its color or create a new color
                Color newColor;

                // first check for service lane area
                if (!NotInServiceLaneWorld(GetWorldFromGrid_X(countX), GetWorldFromGrid_Y(countY)))
                {
                    tileSpriteRenderer.sprite = serviceTile;
                }

                int tileGroup = 0;
                if (crateController.crateGrid[countX, countY] != null)
                {
                    tileGroup = crateController.crateGrid[countX, countY].Group;
                }

                // FIRST find if there is no group

                newColor = new Color(1f, 1f, 1f, 1f);

                if (tileGroup == 0)
                {
                }
                else if (groupToColorMap.ContainsKey(tileGroup)) // the group has a mapped color
                {
                    newColor = paletteArray[groupToColorMap[tileGroup]];
                }
                else
                {
                    if (NotInServiceLaneWorld(countX, countY))
                    {

                    int newColorIndex = SetUpNewColor();
                    newColor = paletteArray[newColorIndex];
                    groupToColorMap.Add(tileGroup, newColorIndex);
                    }
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
        for (int countX = 0; countX < storageWidth; countX++)
        {
            for (int countY = 0; countY < storageHight; countY++)
            {
                if (crateMaster.crateGrid[countX, countY] == null)
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

    internal bool NotInServiceLaneWorld(int posW, int posH)
    {
        if (posW > storageAreaOriginW &&
            posW < storageAreaEndPointW &&
            posH > storageAreaOriginH &&
            posH < storageAreaEndPointH)
        {
            return true;
        }
        return false;
    }

    internal bool NotInServiceLaneGrid(int posX, int posY)
    {
        if (posX > 0 &&
            posX < storageWidth &&
            posY > 0 &&
            posY < storageHight)
        {
            return true;
        }
        return false;
    }

    public bool CreateFloor()
    {

        for (int countH = 0; countH < storageHight; countH++)
        {
            for (int countW = 0; countW < storageWidth; countW++)
            {
                float positionX = countW + storageAreaOriginW;
                float positionY = countH + storageAreaOriginH;
                GameObject newTile = Instantiate(concreteTile, new Vector3(positionX, positionY), Quaternion.identity, tilesParent);
                newTile.name = "tile " + positionX + " / " + positionY;
                // Debug.Log("placed a tile at: " + countX + " / " +  countY);
            }
        }
        return true;
    }

    public bool IsTileAvailableForCrateRel(int coordW, int coordH) // input is Relative
    {
        int playerWidth = (int)(player.transform.position.x - storageAreaOriginW);
        int playerHeight = (int)(player.transform.position.y - storageAreaOriginH);
        if ((coordW != playerWidth) || (coordH != playerHeight))
        {
            return (crateMaster.crateGrid[coordW, coordH] == null);
        }
        else
        {
            return false;
        }
    }

    public bool IsTileEmpty(int coordX, int coordY) // TODO :: Objectify - done
    {
        if (IsWithinBorders(coordX, coordY) && (crateMaster.crateGrid[coordX, coordY] == null))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Converts World to Grid horizontal coordinate
    /// </summary>
    /// <param name="relW">horizontal position in a world view</param>
    /// <returns></returns>
    public int GetGridFromWorld_X(int relW)
    {
        return relW - storageAreaOriginW;
    }

    /// <summary>
    /// Converts World to Grid vertical coordinate
    /// </summary>
    /// <param name="relH">vertical position in a world view</param>
    /// <returns></returns>
    public int GetGridFromWorld_Y(int relH)
    {
        return relH - storageAreaOriginH;
    }

    public int GetWorldFromGrid_X(int relW)
    {
        return relW + storageAreaOriginW;
    }

    public int GetWorldFromGrid_Y(int relH)
    {
        return relH + storageAreaOriginH;
    }

    public bool IsWithinBorders(int posX, int posY)
    {
        if ((posX >= 0 && posX < storageWidth)
            && (posY >= 0 && posY < storageHight))
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

                //PlacePaletteTile(paletteCounter, newColor);

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
        GameObject newPaletteTile = Instantiate(basePaletteTile, new Vector3(anchorX + (fullDeltaX), anchorY), Quaternion.identity, paletteAnchor.GetComponent<Transform>());
        newPaletteTile.GetComponent<SpriteRenderer>().color = color;
        return true;
    }

    public bool PlaceFactories()
    {
        // get the dimensions
        // factoryholder
        float[] factoryPosWorldX = new float[2] { storageAreaOriginW - 0.8f, storageAreaEndPointW + 0.8f };
        float[] factoryPosWorldY = new float[2] { storageAreaEndPointH - 0.8f, storageAreaOriginH + 1.2f };
        //float factoryLeft = storageAreaOriginW - 0.8f;
        //float factoryRight = storageAreaEndPointW + 0.8f;
        //float factoryBottom = storageAreaOriginH + 1.2f;
        //float factoryTop = storageAreaEndPointH - 0.8f;
        int flip = 1;
        int count = 0;
        Transform factoryHolder = GameObject.Find("Factories").GetComponent<Transform>();
        for (int countX = 0; countX < 2; countX++)
        {
            for (int countY = 0; countY < 2; countY++)
            {
                if (countX == 1)
                {
                    flip = -1;
                }
                else
                {
                    flip = 1;
                }
                float factPosX = factoryPosWorldX[countX];
                float factPosY = factoryPosWorldY[countY];
                GameObject newFactoryGO = Instantiate(factoryPrefab, new Vector3(factPosX, factPosY), Quaternion.identity, factoryHolder);
                newFactoryGO.GetComponent<Transform>().localScale = new Vector3(flip, 1, 1);
                Factory newFactoryOBJ = new Factory(factPosX, factPosY, newFactoryGO, crateMaster);
                factoryList[count] = newFactoryOBJ;
                count++;
            }
        }

        return true;
    }
}
