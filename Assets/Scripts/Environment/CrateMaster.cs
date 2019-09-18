using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateMaster : MonoBehaviour
{
    [SerializeField] GameObject crateOne;
    [SerializeField] GameObject crateTwo;
    [SerializeField] GameObject crateThree;
    [SerializeField] GameObject crateFour;
    [SerializeField] GameObject genericCrate;
    [SerializeField] StorageAreaCreator storageCreator;
    [SerializeField] Transform crateHolder;
    float positionW;
    float positionH;

    [SerializeField] public List<GameObject> cratesList; // list of all the crates
    
    // stack structures
    // 1 - a grid tracking which positions are checked
    private bool[,] checkGrid;

    // 2 - a list of groups - may contain up to toal area / 2 number of groups
    private List<List<GameObject>> groupList;

    // Start is called before the first frame update
    void Start()
    {
        cratesList = new List<GameObject>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SpawnCrateAtRandomPsition(int number)
    {
        for (int count = 0; count < number; count++)
        {
            // find random coordinates
            float maxW = storageCreator.storageAreaW-1;
            float maxH = storageCreator.storageAreaH-1;
            int randomW = (int)Mathf.Round(Random.Range(0, maxW));
            int randomH = (int)Mathf.Round(Random.Range(0, maxH));

            while (!storageCreator.IsTileAvailableForCrate(randomW, randomH))
            {
                randomW = randomW = (int)Mathf.Round(Random.Range(0, maxW));
                randomH = randomH = (int)Mathf.Round(Random.Range(0, maxH));
            }

            positionW = randomW + storageCreator.storageAreaOriginW;
            positionH = randomH + storageCreator.storageAreaOriginH;
            Debug.Log("found spot @ " + randomW + " / " + randomH);

            // Choose random crate type
            int crateType = Random.Range(1, 5);

            bool result = CreateCrateByType(crateType, randomW, randomH);

            Debug.Log("crate @ " + randomW + " / " + randomH);
        }

        return true;
    }

    public bool CreateCrateByType(int crateType, float coordW, float coordH)
    {
        GameObject newCrate = Instantiate(NewCrateByType(crateType), new Vector3(coordW, coordH), Quaternion.identity);
        RegisterCrate(newCrate);
        newCrate.transform.SetParent(crateHolder);
        newCrate.name = "crate 0" + crateType;
        int coordAbsW = storageCreator.GetComponent<StorageAreaCreator>().GetRelToAbs_W((int)coordW);
        int coordAbsH = storageCreator.GetComponent<StorageAreaCreator>().GetRelToAbs_H((int)coordH);
        storageCreator.MarkVacancyGrid((int)coordAbsW, (int)coordAbsH, false);
        Debug.Log("crate @ " + coordW + " / " + coordH);

        return true;
    }

    public GameObject NewCrateByType(int crateType)
    {
        GameObject crateToSpawn;
        if (crateType == 1)
        {
            crateToSpawn = crateOne;
        }
        else if (crateType == 2)
        {
            crateToSpawn = crateTwo;
        }
        else if (crateType == 3)
        {
            crateToSpawn = crateThree;
        }
        else
        {
            crateToSpawn = crateFour;
        }

        return crateToSpawn;
    }

    public void RegisterCrate(GameObject newCrate)
    {
        cratesList.Add(newCrate);
    }

    public void EraseCrate(GameObject newCrate)
    {
        cratesList.Remove(newCrate);
    }

    private void BuildGroups()
    {
        // RECURSIVE
        // start from Origin
        // find the first unckecked crate
        // mark all neighbours
        // check all neighbours

        // start group
        // keep track of current group
        // take unchecked cell
        // add to current group
        // put all unchecked neighbours in a queue
        // check all cells in the queue until it is empty
        // change current group
        // find next unchecked cell

    }
}
