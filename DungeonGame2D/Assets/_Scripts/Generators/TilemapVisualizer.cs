using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

// j‰rkytt‰v‰‰ r‰pellyst‰ mutta kaikki toimii
public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    // ƒl‰ edes yrit‰ p‰‰telle nimist‰ mit‰‰n
    private TileBase wallTop, wallTopRight, wallLeftRightLeftRight, wallLeftRightLeft, wallRightRight, wallLeftRightRight, 
        wallTopLeft, wallTopBoth,wallLeftRight,wallLeftRightBottom,wallLeftRightTop, wallLeftLeft, wallUpDownLeft,
        wallSideRight, wallSideLeft, wallBottom, wallBottom2, wallFull, wallTopBottomRight, wallTopBottomLeft,
        wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight, wallUpDownRight,
        cornerDiagonalRight, cornerDiagonalLeft, wallLeftRightTopRight, wallLeftRightTopLeft, wallLeftRightTopBoth,
        wallTRight, wallTLeft, wallRLD, wallLRD, cornerNDL, cornerNDR, cornerNUL, cornerNUR,
        wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft, cornerDownLeftRight;
    [SerializeField]
    private List<TileBase> floorTileList;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTileList);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, List<TileBase> floorTileList)
    {
        foreach (var position in positions)
        {
            TileBase tile = floorTileList[Random.Range(0, floorTileList.Count)];
            PaintSingleTile(tilemap, tile, position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallTRight.Contains(typeAsInt))
        {
            tile = wallTRight;
        }
        else if (WallTypesHelper.wallRLD.Contains(typeAsInt))
        {
            tile = wallRLD;
        }
        else if (WallTypesHelper.wallLRD.Contains(typeAsInt))
        {
            tile = wallLRD;
        }
        else if (WallTypesHelper.wallTLeft.Contains(typeAsInt))
        {
            tile = wallTLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallLeftRightTopBoth.Contains(typeAsInt))
        {
            tile = wallLeftRightTopBoth;
        }
        else if (WallTypesHelper.wallLeftRightTopRight.Contains(typeAsInt))
        {
            tile = wallLeftRightTopRight;
        }
        else if (WallTypesHelper.wallLeftRightTopLeft.Contains(typeAsInt))
        {
            tile = wallLeftRightTopLeft;
        }
        else if (WallTypesHelper.wallLeftRightLeft.Contains(typeAsInt))
        {
            tile = wallLeftRightLeft;
        }
        else if (WallTypesHelper.wallTopBottomRight.Contains(typeAsInt))
        {
            tile = wallTopBottomRight;
        }
        else if (WallTypesHelper.wallUpDownRight.Contains(typeAsInt))
        {
            tile = wallUpDownRight;
        }
        else if (WallTypesHelper.wallUpDownLeft.Contains(typeAsInt))
        {
            tile = wallUpDownLeft;
        }
        else if (WallTypesHelper.wallTopBottomLeft.Contains(typeAsInt))
        {
            tile = wallTopBottomLeft;
        }
        else if (WallTypesHelper.wallLeftRightRight.Contains(typeAsInt))
        {
            tile = wallLeftRightRight;
        }
        else if (WallTypesHelper.wallLeftRightLeftRight.Contains(typeAsInt))
        {
            tile = wallLeftRightLeftRight;
        }        
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallTopLeft.Contains(typeAsInt))
        {
            tile = wallTopLeft;
        }
        else if (WallTypesHelper.wallTopBoth.Contains(typeAsInt))
        {
            tile = wallTopBoth;
        }
        else if (WallTypesHelper.wallTopRight.Contains(typeAsInt))
        {
            tile = wallTopRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallLeftRight.Contains(typeAsInt))
        {
            tile = wallLeftRight;
        }
        else if (WallTypesHelper.wallLeftRightBottom.Contains(typeAsInt))
        {
            tile = wallLeftRightBottom;
        }
        else if (WallTypesHelper.wallLeftRightTop.Contains(typeAsInt))
        {
            tile = wallLeftRightTop;
        }

        if (tile!=null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallRightRight.Contains(typeAsInt))
        {
            tile = wallRightRight;
        }
        else if (WallTypesHelper.cornerNUL.Contains(typeAsInt))
        {
            tile = cornerNUL;
        }
        else if (WallTypesHelper.cornerNUR.Contains(typeAsInt))
        {
            tile = cornerNUR;
        }
        else if (WallTypesHelper.cornerNDR.Contains(typeAsInt))
        {
            tile = cornerNDR;
        }
        else if (WallTypesHelper.cornerNDL.Contains(typeAsInt))
        {
            tile = cornerNDL;
        }
        else if (WallTypesHelper.cornerDiagonalRight.Contains(typeAsInt))
        {
            tile = cornerDiagonalRight;
        }
        else if (WallTypesHelper.cornerDiagonalLeft.Contains(typeAsInt))
        {
            tile = cornerDiagonalLeft;
        }
        else if (WallTypesHelper.cornerDownLeftRight.Contains(typeAsInt))
        {
            tile = cornerDownLeftRight;
        }
        else if (WallTypesHelper.wallLeftLeft.Contains(typeAsInt))
        {
            tile = wallLeftLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position);
    }
}
