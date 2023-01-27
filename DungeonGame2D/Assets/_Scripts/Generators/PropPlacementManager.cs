using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PropPlacementManager : MonoBehaviour
{
    DungeonData dungeonData;

    [SerializeField]
    private List<Prop> propsToPlace;

    [SerializeField]
    private List<RoomSettigns> roomsToFill;

    private RoomSettigns roomSettings = null;

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;

    private HashSet<Vector2Int> propPlacements;

    private void Awake()
    {
        dungeonData = FindObjectOfType<DungeonData>();
    }

    public void ProcessRooms()
    {
        propPlacements = new HashSet<Vector2Int>();
        if (dungeonData == null)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            foreach(RoomSettigns roomSet in roomsToFill)
            {
                if (room.RoomType == roomSet.name)
                    roomSettings = roomSet;
            }
            if (roomSettings == null)
            {
                Debug.Log("Error: RoomTypeNull");
                continue;
            }

            //Uutta
            List<Prop> roomProps = roomSettings.propsToPlace;
            List<Prop> obstcleProps = new List<Prop>();
            List<Prop> crateProps = new List<Prop>();
            List<Prop> treasureProps = new List<Prop>();
            List<Prop> torchProps = new List<Prop>();
            foreach (Prop prop in roomProps)
            {
                if (prop.propType == "obstacle")
                {
                    obstcleProps.Add(prop);
                }
                else if (prop.propType == "crate")
                {
                    crateProps.Add(prop);
                }
                else if (prop.propType == "treasure")
                {
                    treasureProps.Add(prop);
                }
                else if (prop.propType == "torch")
                {
                    torchProps.Add(prop);
                }
            }
            int obstacleQuantity
                    = UnityEngine.Random.Range(roomSettings.ObstacleQuantityMin, roomSettings.ObstacleQuantityMax + 1);
            int crateQuantity
                    = UnityEngine.Random.Range(roomSettings.CrateQuantityMin, roomSettings.CrateQuantityMax + 1);
            int treasureQuantity
                    = UnityEngine.Random.Range(roomSettings.TreasureQuantityMin, roomSettings.TreasureQuantityMax + 1);
            int torchQuantity
                    = UnityEngine.Random.Range(roomSettings.TorchQuantityMin, roomSettings.TorchQuantityMax + 1);
            PlacePropOfType(room, obstcleProps, obstacleQuantity);
            PlacePropOfType(room, treasureProps, treasureQuantity);
            PlacePropOfType(room, crateProps, crateQuantity);
            PlacePropOfType(room, torchProps, torchQuantity);
            if (room.RoomType == "bossRoom")
            {
                foreach(Prop prop in propsToPlace)
                {
                    if (prop.name == "exit")
                    {
                        PlacePropGameObjectAt(room, room.RoomCenterPos, prop);
                    }
                }
                
            }


        }

        //OnFinished?.Invoke();
        Invoke("RunEvent", 1);

    }

    private void PlacePropOfType(Room room, List<Prop> props, int quantity)
    {
        for (int i = 0; i < quantity; i++)

        {
            if (props.Count == 0)
            {
                continue;
            }
            Prop prop = props[UnityEngine.Random.Range(0, props.Count)];
            HashSet<Vector2Int> possiblePositions = new HashSet<Vector2Int>();
            if (prop.Corner)
            {
                possiblePositions.UnionWith(room.CornerTiles);
            }
            if (prop.NearWallDown)
            {
                possiblePositions.UnionWith(room.NearWallTilesDown);
            }
            if (prop.NearWallLeft)
            {
                possiblePositions.UnionWith(room.NearWallTilesLeft);
            }
            if (prop.NearWallRight)
            {
                possiblePositions.UnionWith(room.NearWallTilesRight);
            }
            if (prop.NearWallUP)
            {
                possiblePositions.UnionWith(room.NearWallTilesUp);
            }
            if (prop.Inner)
            {
                possiblePositions.UnionWith(room.InnerTiles);
            }
            possiblePositions.ExceptWith(room.PropPositions);
            possiblePositions.ExceptWith(dungeonData.Path);
            List<Vector2Int> availablePositions = possiblePositions.OrderBy(x => Guid.NewGuid()).ToList();
            TryPlacingPropBruteForce(room, prop, availablePositions, PlacementOriginCorner.BottomLeft);
        }
    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    /// <summary>
    /// Tries to place the Prop using brute force (trying each available tile position)
    /// </summary>
    /// <param name="room"></param>
    /// <param name="propToPlace"></param>
    /// <param name="availablePositions"></param>
    /// <param name="placement"></param>
    /// <returns>False if there is no space. True if placement was successful</returns>
    private bool TryPlacingPropBruteForce(
        Room room, Prop propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //try placing the objects starting from the corner specified by the placement parameter
        for (int i = 0; i < availablePositions.Count; i++)
        {
            //select the specified position (but it can be already taken after placing the corner props as a group)
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //check if there is enough space around to fit the prop
            List<Vector2Int> freePositionsAround
                = TryToFitProp(propToPlace, availablePositions, position, placement);

            //If we have enough spaces place the prop
            if (freePositionsAround.Count == propToPlace.PropSize.x * propToPlace.PropSize.y)
            {
                //Place the gameobject
                if (propPlacements.Contains(position) == false)
                {
                    PlacePropGameObjectAt(room, position, propToPlace);
                    //Lock all the positions recquired by the prop (based on its size)
                    foreach (Vector2Int pos in freePositionsAround)
                    {
                        //Hashest will ignore duplicate positions
                        room.PropPositions.Add(pos);
                        propPlacements.Add(pos);

                    }
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the prop will fit (accordig to it size)
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="availablePositions"></param>
    /// <param name="originPosition"></param>
    /// <param name="placement"></param>
    /// <returns></returns>
    private List<Vector2Int> TryToFitProp(
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        //Perform the specific loop depending on the PlacementOriginCorner
        if (placement == PlacementOriginCorner.BottomLeft)
        {
            for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.BottomRight)
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.TopLeft)
        {
            for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }

        return freePositions;
    }

   
    /// <summary>
    /// Place a prop as a new GameObject at a specified position
    /// </summary>
    /// <param name="room"></param>
    /// <param name="placementPostion"></param>
    /// <param name="propToPlace"></param>
    /// <returns></returns>
    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPostion, Prop propToPlace)
    {
        //Instantiat the prop at this positon
        GameObject prop = Instantiate(propPrefab);
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();
        Light2D light = prop.GetComponentInChildren<Light2D>();

        //set the sprite
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        //Add a collider
        if (propToPlace.hasCollider)
        {
            CapsuleCollider2D collider
                = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
            collider.offset = Vector2.zero;
            if (propToPlace.PropSize.x > propToPlace.PropSize.y)
            {
                collider.direction = CapsuleDirection2D.Horizontal;
            }
            Vector2 size
                = new Vector2(propToPlace.PropSize.x * 0.8f, propToPlace.PropSize.y * 0.8f);
            collider.size = size;
        }
        prop.transform.localPosition = (Vector2)placementPostion;
        //adjust the position to the sprite
        propSpriteRenderer.transform.localPosition
            = (Vector2)propToPlace.PropSize * 0.5f;

        if (propToPlace.isLight)
        {
            light.enabled = true;
        }
        else
        {
            light.enabled = false;
        }

        // S‰‰det‰‰n HP
        prop.GetComponentInChildren<Health>().InitializeHealth(propToPlace.health);
        if (propToPlace.health == 0)
            prop.GetComponentInChildren<Health>().canDie = false;


        // Asetetaan animaatiot
        if (propToPlace.hasAnimation)
        {
            Animator animator = prop.GetComponentInChildren<Animator>();
            if(propToPlace.name == "torch")
            {
                if (CheckIfWall(placementPostion, Vector2Int.up))
                {
                    animator.SetBool("isTorch", true);
                }else if (CheckIfWall(placementPostion, Vector2Int.left))
                {
                    animator.SetBool("isTorchLeft", true);
                }
                else if (CheckIfWall(placementPostion, Vector2Int.right))
                {
                    animator.SetBool("isTorchRight", true);
                }
                else if (CheckIfWall(placementPostion, Vector2Int.down))
                {
                    animator.SetBool("isTorchDown", true);
                }
                else
                {
                    // Se on generoitunut v‰‰rin niin poistetaan sprite
                    propSpriteRenderer.enabled = false;
                    light.enabled = false;
                    return prop;

                }

            }
            else if (propToPlace.name == "bossRoomTorch")
            {
                animator.SetBool("isBossRoomTorch", true);
                light.color = new Color32(225, 130, 112,225);
            }else if (propToPlace.name == "smallCrate")
            {
                animator.SetBool("isSmallCrate", true);
            }
            else if (propToPlace.name == "largeCrate")
            {
                animator.SetBool("isLargeCrate", true);
            }
            animator.enabled = true;
        }

        //Save the prop in the room data (so in the dunbgeon data)
        room.PropPositions.Add(placementPostion);
        propPlacements.Add(placementPostion);
        room.PropObjectReferences.Add(prop);
        return prop;
    }

    private bool CheckIfWall(Vector2Int placementPostion, Vector2Int direction)
    {
        if (dungeonData.FloorTiles.Contains(placementPostion + direction))
            return false;
        return true;
    }
}

/// <summary>
/// Where to start placing the prop ex. start at BottomLeft corner and search 
/// if there are free space to the Right and Up in case of placing a biggex prop
/// </summary>
public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}
