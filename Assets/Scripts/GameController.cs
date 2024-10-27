using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Scores")]
    [SerializeField] public int organicScore;
    [SerializeField] public bool isOrganicContaminated;
    [SerializeField] public int trashScore;
    [SerializeField] public int recyclingScore;
    [SerializeField] public bool isRecyclingContaminated;

    [Header("Item Interaction")]
    public LayerMask itemLayer;
    [Range(0.0f, 100.0f)]
    public float itemDamping = 1.0f;
    [Range(0.0f, 100.0f)]
    public float itemFrequency = 5.0f;
    private TargetJoint2D itemJoint;

    [Header("Game Settings")]
    [SerializeField] public bool gameStarted = false;
    [SerializeField] public float gameLength = 30f;
    [SerializeField] public float timeRemaining;
    [SerializeField] private float zoomElapsed;
    [SerializeField] private float zoomDuration;
    [SerializeField] private UIController UIController;
    [SerializeField] private ItemGenerator itemGenerator;


    // Start is called before the first frame update
    void Start()
    {
        isOrganicContaminated = false;
        isRecyclingContaminated = false;
        zoomDuration = 2;
        timeRemaining = gameLength;
        UIController = Object.FindObjectOfType<UIController>();
        itemGenerator = Object.FindObjectOfType<ItemGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted && timeRemaining > 0f) {  return; }
        else if (timeRemaining <= 0f) { EndGame();  }

        // Calculate the world position for the mouse.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Attach joint to item player clicked on
        if (Input.GetMouseButtonDown(0))
        {
            // Get item collider that overlaps with mouse position
            Collider2D collider = Physics2D.OverlapPoint(mousePos, itemLayer);
            if (!collider) { return; }

            // Add a target joint to the item's rb
            itemJoint = collider.attachedRigidbody.gameObject.AddComponent<TargetJoint2D>();
            itemJoint.dampingRatio = itemDamping;
            itemJoint.frequency = itemFrequency;

            // Attach the anchor to the local-point where we clicked.
            itemJoint.anchor = itemJoint.transform.InverseTransformPoint(mousePos);
        }
        // Release joint on item when player releases mouse
        else if (Input.GetMouseButtonUp(0))
        {
            Destroy(itemJoint);
            itemJoint = null;
            return;
        }

        // Move joint with mouse while dragging
        if (itemJoint)
        {
            itemJoint.target = mousePos;
        }
    }

    public void UpdateScore(string itemScore, string itemType)
    {
        if (itemScore != itemType && itemScore != "trash")
        {
            switch (itemScore)
            {
                case "organics":
                    isOrganicContaminated = true;
                    break;
                case "recycling":
                    isRecyclingContaminated = true;
                    break;
                default:
                    break;
            }
        }
        else
        {
            IncrementScore(itemScore);
        }
    }

    public void StartGame()
    {
        itemGenerator.Generate();
        gameStarted = true;
        UIController.startCanvas.SetActive(false);
    }

    public void EndGame()
    {
        gameStarted = false;

        // Return early and try again if not all items have been scored
        ItemBehaviour[] items = Object.FindObjectsOfType<ItemBehaviour>();
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].isScored)
            {
                return;
            }
        }

        // Zoom out camera and reveal scores
        if (zoomElapsed < zoomDuration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(5, 20, zoomElapsed / zoomDuration);
            zoomElapsed += Time.deltaTime;
        }
        else
        {
            UIController.RevealScore();
        }
    }

    private void IncrementScore(string itemScore)
    {
        switch (itemScore)
        {
            case "organics":
                organicScore++;
                break;
            case "trash":
                trashScore++;
                break;
            case "recycling":
                recyclingScore++;
                break;
            default:
                break;
        }
    }
}
