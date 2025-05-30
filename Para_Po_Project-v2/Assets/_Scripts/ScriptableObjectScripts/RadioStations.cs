using UnityEngine;

[CreateAssetMenu(fileName = "New Radio Station", menuName = "Radio Station")]
public class RadioStations : ScriptableObject
{
    [SerializeField] private AudioClip[] audioTracks;

    public AudioClip[] Tracks { get { return audioTracks; } }
}
