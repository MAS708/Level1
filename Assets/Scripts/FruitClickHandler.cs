using UnityEngine;

public class FruitClickHandler : MonoBehaviour
{
    private FruitManager fruitManager;
    private SpriteRenderer spriteRenderer;

    private bool canClick = true;

    private void Start()
    {
        fruitManager = FindObjectOfType<FruitManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (canClick)
        {
            string fruitName = spriteRenderer.sprite.name;
            fruitManager.OnFruitClick(fruitName);
        }
    }

    public void EnableClick()
    {
        canClick = true;
    }

    public void DisableClick()
    {
        canClick = false;
    }
}
