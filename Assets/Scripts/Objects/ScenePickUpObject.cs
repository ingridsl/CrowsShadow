﻿using UnityEngine;

public class ScenePickUpObject : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;
    public SceneObject.PositionSprite positionSprite;
    public float scale = 1;
    public Inventory.InventoryItems item;
    public bool colliding = false;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    float sizeX, sizeY;
    float posX, posY, posXdefault, posYdefault;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = sprite1;
        sizeX = boxCollider.size.x / spriteRenderer.bounds.size.x;
        sizeY = boxCollider.size.y / spriteRenderer.bounds.size.y;
        posX = spriteRenderer.bounds.size.x / scale;
        posY = spriteRenderer.bounds.size.y / scale;
        posYdefault = transform.position.y;
        posXdefault = transform.position.x;
    }

    void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        if (Input.GetKeyDown(KeyCode.Z) && colliding &&
            !MissionManager.instance.paused && !MissionManager.instance.blocked &&
            !MissionManager.instance.pausedObject) //GetKeyDown e GetKeyUp não pode ser usado fora do Update
        {
            ChangeSprite();
        }
    }

    void ChangeSprite()
    {
        print("SceneObject");
        if (spriteRenderer.sprite == sprite1)
        {
            spriteRenderer.sprite = sprite2;
            Inventory.NewItem(item);
        }
        else
        {
            spriteRenderer.sprite = sprite1;
        }

        if (positionSprite == SceneObject.PositionSprite.LEFT && spriteRenderer.sprite == sprite2)
        {
            transform.position = new Vector3(transform.position.x + posX, posYdefault, transform.position.z);
        }
        else if (positionSprite == SceneObject.PositionSprite.RIGHT && spriteRenderer.sprite == sprite2)
        {
            transform.position = new Vector3(transform.position.x - posX, posYdefault, transform.position.z);
        }
        else if (positionSprite == SceneObject.PositionSprite.UP && spriteRenderer.sprite == sprite2)
        {
            transform.position = new Vector3(posXdefault, transform.position.y + posY, transform.position.z);
        }
        else if (positionSprite == SceneObject.PositionSprite.DOWN && spriteRenderer.sprite == sprite2)
        {
            transform.position = new Vector3(posXdefault, transform.position.y - posY, transform.position.z);
        }
        else if (positionSprite != SceneObject.PositionSprite.DEFAULT)
        {
            transform.position = new Vector3(posXdefault, posYdefault, transform.position.z);
        }

        boxCollider.size = new Vector2(
            sizeX * spriteRenderer.bounds.size.x,
            sizeY * spriteRenderer.bounds.size.y);

    }

}