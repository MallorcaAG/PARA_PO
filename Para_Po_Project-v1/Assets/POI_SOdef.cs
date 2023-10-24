using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Points Of Interest", menuName = "ScriptableObjects/PointsOfInterest", order = 1)]
public class POI_SOdef : ScriptableObject
{
    public Transform[] POIs;
}
