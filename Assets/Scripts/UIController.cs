using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject recyclingWall;
    [SerializeField] private GameObject trashWall;
    [SerializeField] private bool initialScoreRevealed = false;

    [Header("UI References")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private GameObject trashText;
    [SerializeField] private GameObject[] itemScores;
    [SerializeField] private GameObject organicText;
    [SerializeField] private GameObject recyclingText;
    [SerializeField] private GameObject restartButton;

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
        // If already revealed, return early
        if (initialScoreRevealed) { return; }
        restartButton.SetActive(false);
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

            initialScoreRevealed = true;
            itemScore.GetComponent<TMP_Text>().SetText(score);
        }

        if (gameController.isRecyclingContaminated || gameController.isOrganicContaminated)
        {
            Invoke("RevealContamination", 5f);
        } 
        else
        {
            Invoke("RevealRecyclingCut", 5f);
        }
    }

    public void ResetUIs()
    {
        startCanvas.SetActive(false);
        endCanvas.SetActive(false);
        initialScoreRevealed = false;
        organicText.GetComponent<TMP_Text>().SetText("Single Use Items Composted:");
        recyclingText.GetComponent<TMP_Text>().SetText("Single Use Items Recycled:");
    }

    private void RevealContamination()
    {
        if (gameController.isOrganicContaminated)
        {
            organicText.GetComponent<TMP_Text>().SetText("Unfortunately, your organics are contaminated with inorganics:");
            for (int i = 0; i < itemScores.Length; i++)
            {
                GameObject itemScore = itemScores[i];

                if (itemScore.tag == "organics")
                {
                    itemScore.GetComponent<TMP_Text>().SetText("0");
                }
            }
        }
        
        if (gameController.isRecyclingContaminated)
        {
            recyclingText.GetComponent<TMP_Text>().SetText("Unfortunately, your recyclables are contaminated with non-recyclables:");
            for (int i = 0; i < itemScores.Length; i++)
            {
                GameObject itemScore = itemScores[i];

                if (itemScore.tag == "recycling")
                {
                    itemScore.GetComponent<TMP_Text>().SetText("0");
                }
            }
            Invoke("AllowRestart", 3f);
        }
        else {
            Invoke("RevealRecyclingCut", 5f);
        }
    }

    private void RevealRecyclingCut()
    {
        // If already revealed, return early
        if (restartButton.activeSelf) { return; }

        recyclingText.GetComponent<TMP_Text>().SetText("Unfortunately, only ~35% of recyclables are actually recycled:");
        recyclingWall.SetActive(false);
        trashWall.SetActive(false);
        for (int i = 0; i < itemScores.Length; i++)
        {
            GameObject itemScore = itemScores[i];
            int updatedScore = Mathf.FloorToInt(gameController.recyclingScore * 0.35f);

            if (itemScore.tag == "recycling")
            {
                itemScore.GetComponent<TMP_Text>().SetText(updatedScore.ToString());
            }
        }
        Invoke("AllowRestart", 3f);
    }

    private void AllowRestart()
    {
        recyclingWall.SetActive(true);
        trashWall.SetActive(true);
        restartButton.SetActive(true);
    }
}
