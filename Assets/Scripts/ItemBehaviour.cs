using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [Header("Item Scoring")]
    [SerializeField] public bool isScored;
    [SerializeField] private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        isScored = false;
        gameController = Object.FindObjectOfType<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isScored)
        {
            gameController.UpdateScore(collision.tag, this.tag);
            isScored = true;
        }
    }
}
