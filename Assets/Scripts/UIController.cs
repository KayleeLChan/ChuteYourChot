using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] public GameObject startCanvas;
    [SerializeField] public GameObject endCanvas;
    [SerializeField] private GameObject[] itemScores;
    [SerializeField] private GameObject organicText;
    [SerializeField] private GameObject recyclingText;
    [SerializeField] private GameObject trashText;

    // Start is called before the first frame update
    void Start()
    {
        gameController = Object.FindObjectOfType<GameController>();
        endCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RevealScore()
    {
        endCanvas.SetActive(true);
        for (int i = 0; i < itemScores.Length; i++) {
            GameObject itemScore = itemScores[i];
            string score;
            string currScore = itemScore.GetComponent<TMP_Text>().text;

            switch (itemScore.tag)
            {
                case "organics":
                    score = gameController.organicScore.ToString();
                    break;
                case "recycling":
                    score = gameController.recyclingScore.ToString();
                    break;
                default:
                    score = gameController.trashScore.ToString();
                    break;
            }

            itemScore.GetComponent<TMP_Text>().SetText(currScore);
            //StartCoroutine(CountScore(currScore, score, itemScore));
        }

        if (gameController.isRecyclingContaminated && gameController.isOrganicContaminated)
        {
            Delay(2f);
            RevealDoubleContamination();
        }
        else if (gameController.isRecyclingContaminated || gameController.isOrganicContaminated)
        {
            Delay(2f);
            RevealSingleContamination();
        } 
        else
        {
            RevealRecyclingCut();
        }
    }

    public void RevealDoubleContamination()
    {

    }

    public void RevealSingleContamination()
    {
        if (gameController.isOrganicContaminated)
        {
            organicText.GetComponent<TMP_Text>().SetText("Unfortunately, your organics are contaminated with inorganics");
            for (int i = 0; i < itemScores.Length; i++)
            {
                GameObject itemScore = itemScores[i];

                if (itemScore.tag == "organics")
                {
                    itemScore.GetComponent<TMP_Text>().SetText("0");
                }
            }
            Delay(2f);
            RevealRecyclingCut();
        }
        else
        {
            recyclingText.GetComponent<TMP_Text>().SetText("Unfortunately, your recyclables are contaminated with non-recyclables");
            for (int i = 0; i < itemScores.Length; i++)
            {
                GameObject itemScore = itemScores[i];

                if (itemScore.tag == "recycling")
                {
                    itemScore.GetComponent<TMP_Text>().SetText("0");
                }
            }
        }
    }

    public void RevealRecyclingCut()
    {
        Delay(2f);
        recyclingText.GetComponent<TMP_Text>().SetText("Unfortunately, only ~35% of recyclables are actually recycled");
        for (int i = 0; i < itemScores.Length; i++)
        {
            GameObject itemScore = itemScores[i];
            int updatedScore = Mathf.FloorToInt(gameController.recyclingScore * 0.35f);

            if (itemScore.tag == "recycling")
            {
                itemScore.GetComponent<TMP_Text>().SetText(updatedScore.ToString());
            }
        }
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    IEnumerator CountScore(string currScore, string score, GameObject itemScore)
    {
        while (currScore != score)
        {
            int incrementedScore = int.Parse(currScore) + 1;
            currScore = incrementedScore.ToString();
            itemScore.GetComponent<TMP_Text>().SetText(currScore);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
