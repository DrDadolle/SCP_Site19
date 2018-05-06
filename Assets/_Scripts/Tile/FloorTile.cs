﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/**
 * Default Tile for the world
 * Contains floorSprite and furnitures
 */
[CreateAssetMenu(menuName = "Tile/Floor Tile")]
public class FloorTile : TileBase
{
    //The sprite of tile in the palette
    public Sprite SpriteOfFloor;
    public Sprite PendingSprite;

    //Is this a pending tile ?
    public bool isPending;


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (isPending)
        {
            tileData.sprite = PendingSprite;
        }
        else
        {
            tileData.sprite = SpriteOfFloor;
        }

        base.GetTileData(position, tilemap, ref tileData);
    }

}
