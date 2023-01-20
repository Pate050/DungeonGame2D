using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LocationDecider
{
    DungeonData dungeonData;

    [SerializeField]
    private List<Prop> propsToPlace;

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinishedFitting;

    private void Awake()
    {
        dungeonData = UnityEngine.Object.FindObjectOfType<DungeonData>();
    }

}