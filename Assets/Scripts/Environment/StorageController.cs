using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class StorageController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SpriteTileMode floorTile;
    [SerializeField] public int storageAreaW;
    [SerializeField] public int storageAreaH;
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

    CrateController crateController;

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
    [SerializeField] GameObject factoryHolder;
    [SerializeField] GameObject basicFactory;
    public Dictionary<int, GameObject> factoryList;
    public List<int> factoryMap;

    // minimum size of grid - TEMP
    int storageMinW = 7;
    int storageMinH = 5;

    void Awake()
    {
        // build refs
        crateController = GameObject.FindObjectOfType<CrateController>();

        factoryList = new Dictionary<int, GameObject>();
        factoryMap = new List<int>();

        // TEMP - area size
        storageAreaW = Mathf.Max(storageAreaW, storageMinW);
        storageAreaH = Mathf.Max(storageAreaH, storageMinH);

        storageAreaOriginW = (storageAreaW / 2) * -1;
        storageAreaOriginH = (storageAreaH / 2) * -1;
        storageAreaEndPointW = storageAreaW + storageAreaOriginW - 1;
        storageAreaEndPointH = storageAreaH + storageAreaOriginH - 1;

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
        crateController = FindObjectOfType<CrateController>();
        crateMasterTransform = crateController.GetComponent<Transform>();

        PlaceFactories();

        // spawn incoming crates
        InitializeFactoryColors();
    }

    // Update is called once per frame
    private void Update()
    {
        // highlight groups according to tiles groups
        RecolorGrid();

    }


    private void InitializeFactoryColors()
    {
        // build factory colors
        Color[] colorPool = new Color[]
            {
                new Color(0.5f, 0.4f, 0.0f, 1f),
                new Color(1, 1, 0, 1),
                new Color(1, 0, 0, 1),
                new Color(0, 1, 0, 1),
                new Color(0, 0, 1, 1),
                new Color(1, 0, 1, 1)
            };

        // NEW VERSION

        for (int factCount = 0; factCount < 4; factCount++)
        {
            int randomType = crateController.GetRandomType();
            factoryMap.Add(randomType);
            factoryList[factCount].GetComponent<SpriteRenderer>().color = colorPool[randomType];
        }

        // OLD VERSION
        //// check if the positions are available
        //// center based positions
        //int leftW = storageAreaOriginW;
        //int rightW = storageAreaEndPointW;
        //int downH = storageAreaOriginH + 1;
        //int upH = storageAreaEndPointH - 1;
        //// origin basec positions
        //int leftX = GetAbsFromRelW(storageAreaOriginW);
        //int rightX = GetAbsFromRelW(storageAreaEndPointW);
        //int downY = GetAbsFromRelH(storageAreaOriginH) + 1;
        //int upY = GetAbsFromRelH(storageAreaEndPointH) - 1;
        //// pick a random type
        //int crateType = PickARandomCrate();
        //if (IsTileAvailableForCrateRel(leftX, upY))
        //{
        //    //bool crateLU = crateController.CreateCrateByType(crateType, (float)leftX, (float)upY);
        //    crateController.CreateCrateByType(crateType, (float)leftW, (float)upH);
        //}
        //if (IsTileAvailableForCrateRel(rightX, upY))
        //{
        //    //bool crateRU = crateController.CreateCrateByType(crateType, (float)rightX, (float)upY);
        //    crateController.CreateCrateByType(crateType, (float)rightW, (float)upH);
        //}
        //if (IsTileAvailableForCrateRel(leftX, downY))
        //{
        //    //bool crateLD = crateController.CreateCrateByType(crateType, (float)leftX, (float)downY);
        //    crateController.CreateCrateByType(crateType, (float)leftW, (float)downH);
        //}
        //if (IsTileAvailableForCrateRel(rightX, downY))
        //{
        //    //bool crateRD = crateController.CreateCrateByType(crateType, (float)rightX, (float)downY);
        //    crateController.CreateCrateByType(crateType, (float)rightW, (float)downH);
        //}

    }

    private void RecolorGrid()
    {
        CrateController crateController = crateMasterTransform.GetComponent<CrateController>();
        for (int countY = 0; countY < storageAreaH; countY++)
        {
            for (int countX = 0; countX < storageAreaW; countX++)
            {
                SpriteRenderer tileSpriteRenderer = FindTileAt(countX, countY).GetComponent<SpriteRenderer>();
                // find if the group exists in the groupToColor map - either assign its color or create a new color
                Color newColor;

                // first check for service lane area
                if (!NotInServiceLaneWorld(GetRelFromAbsW(countX), GetRelFromAbsH(countY)))
                {
                    tileSpriteRenderer.sprite = serviceTile;
                }

                //if (countX == 0 || 
                //    countX == storageAreaW - 1 || 
                //    countY == 0 || 
                //    countY == storageAreaH - 1)
                //{
                //}
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
        for (int countX = 0; countX < storageAreaW; countX++)
        {
            for (int countY = 0; countY < storageAreaH; countY++)
            {
                if (crateController.crateGrid[countX, countY] == null)
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
            posX < storageAreaW &&
            posY > 0 &&
            posY < storageAreaH)
        {
            return true;
        }
        return false;
    }

    public bool CreateFloor()
    {

        for (int countH = 0; countH < storageAreaH; countH++)
        {
            for (int countW = 0; countW < storageAreaW; countW++)
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
            return (crateController.crateGrid[coordW, coordH] == null);
        }
        else
        {
            return false;
        }
    }

    public bool IsTileEmpty(int coordX, int coordY) // TODO :: Objectify - done
    {
        if (IsWithinBorders(coordX, coordY) && (crateController.crateGrid[coordX, coordY] == null))
        {
            return true;
        }
        return false;
    }

    public int GetAbsFromRelW(int relW)
    {
        return relW - storageAreaOriginW;
    }

    public int GetAbsFromRelH(int relH)
    {
        return relH - storageAreaOriginH;
    }

    public int GetRelFromAbsW(int relW)
    {
        return relW + storageAreaOriginW;
    }

    public int GetRelFromAbsH(int relH)
    {
        return relH + storageAreaOriginH;
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
        float factoryLeft = storageAreaOriginW - 0.8f;
        float factoryRight = storageAreaEndPointW + 0.8f;
        float factoryBottom = storageAreaOriginH + 1.2f;
        float factoryTop = storageAreaEndPointH - 0.8f;
        

        GameObject factoryBL = PlaceAFactory(factoryLeft, factoryBottom, 1);
        GameObject factoryTL = PlaceAFactory(factoryLeft, factoryTop, 1);
        GameObject factoryBR = PlaceAFactory(factoryRight, factoryBottom, -1);
        GameObject factoryTR = PlaceAFactory(factoryRight, factoryTop, -1);
        factoryList.Add(0, factoryBL);
        factoryList.Add(1, factoryBR);
        factoryList.Add(2, factoryTL);
        factoryList.Add(3, factoryTR);

        return true;
    }

    GameObject PlaceAFactory(float coordX, float coordY, int flip)
    {
        Transform factoryHolder = GameObject.Find("Factories").GetComponent<Transform>();
        GameObject newFactory = Instantiate(basicFactory, new Vector3(coordX, coordY), Quaternion.identity, factoryHolder);
        newFactory.GetComponent<Transform>().localScale = new Vector3(flip, 1, 1);
        return newFactory;
    }
}
