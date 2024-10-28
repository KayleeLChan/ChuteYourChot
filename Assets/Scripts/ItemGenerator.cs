using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    [Header("Spawn Settings")]
    [SerializeField] public int phase = 0;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float timeBetweenSpawns = 3f;
    [SerializeField] private int numberItemsSpawned = 1;
    [SerializeField] private int[] spawnedPositions;

    [Header("Items")]
    [SerializeField] private GameObject[] organics;
    [SerializeField] private GameObject[] trash;
    [SerializeField] private GameObject[] recyclables;


    // Start is called before the first frame update
    void Start()
    {
        gameController = Object.FindObjectOfType<GameController>();

        if (timeBetweenSpawns < 2)
        {
            Debug.LogAssertion("Time between spawns must be at least 2f!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.gameStarted) { return; }

        if (gameController.timeRemaining > 0f)
        {
            gameController.timeRemaining -= Time.deltaTime;
        }

        if (gameController.timeRemaining <= gameController.gameLength * 0.25 && phase < 3)
        {
            timeBetweenSpawns--;
            phase = 3;
        }
        else if (gameController.timeRemaining <= gameController.gameLength * 0.5 && phase < 2)
        {
            timeBetweenSpawns -= 0.5f;
            numberItemsSpawned++;
            phase = 2;
        }
        else if (gameController.timeRemaining <= gameController.gameLength * 0.75 && phase < 1)
        {
            timeBetweenSpawns -= 0.5f;
            numberItemsSpawned++;
            phase = 1;
        }
        else if (phase == 0)
        {
            timeBetweenSpawns = 3f;
            numberItemsSpawned = 1;
        }
    }

    public void Generate()
    {
        StartCoroutine(SpawnItems());
    }

    private int SpawnItem(GameObject[] items)
    {
        int randomIndex = Random.Range(0, items.Length);
        GameObject randomItem = items[randomIndex];

        // Spawn the item randomly outside the screen in a position that already does not have an item spawned there
        int scaledRandomX;
        do
        {
            scaledRandomX = Random.Range(-800, 900);
        } while (spawnedPositions.Contains(scaledRandomX));
        float randomX = scaledRandomX / 100;
        Instantiate(randomItem, new Vector2(randomX, 7f), Quaternion.Euler(0, 0, Random.Range(0, 361)), spawnParent);
        return scaledRandomX;
    }

    IEnumerator SpawnItems()
    {
        while (gameController.timeRemaining > 0f)
        {
            spawnedPositions = new int[numberItemsSpawned];
            for (int i = 0; i < numberItemsSpawned; i++)
            {
                // Randomly choose which type of item gets spawned and record its position
                int randomItemType = Random.Range(0, 3);
                switch (randomItemType)
                {
                    case 0:
                        spawnedPositions[i] = SpawnItem(organics);
                        break;
                    case 1:
                        spawnedPositions[i] = SpawnItem(recyclables);
                        break;
                    default:
                        spawnedPositions[i] = SpawnItem(trash);
                        break;
                }

            }
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
