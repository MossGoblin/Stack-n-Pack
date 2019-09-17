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


    // Start is called before the first frame update
    void Start()
    {
        cratesList = new List<GameObject>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SpawnCrate(int number)
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

            GameObject crateToSpawn;
            // Choose random crate type
            int crateType = Random.Range(1, 5);
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
            GameObject newCrate = Instantiate(crateToSpawn, new Vector3(positionW, positionH), Quaternion.identity);

            cratesList.Add(newCrate);
            newCrate.transform.SetParent(crateHolder);
            newCrate.name = "crate 0" + crateType;
            storageCreator.MarkVacantyGrid((int)randomW, (int)randomH);
            Debug.Log("crate @ " + randomW + " / " + randomH);
        }

        return true;
    }
}
