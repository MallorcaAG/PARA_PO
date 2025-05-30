using UnityEngine;

[CreateAssetMenu(fileName = "New Phase", menuName = "Traffic Light Phase")]
public class TrafficLightPhases : ScriptableObject
{
    public bool northLeft, northStraight, northRight;
    public bool southLeft, southStraight, southRight;
    public bool eastLeft, eastStraight, eastRight;
    public bool westLeft, westStraight, westRight;
    [Space]
    public bool isPlayerRed;

    public float phaseDuration;
}
