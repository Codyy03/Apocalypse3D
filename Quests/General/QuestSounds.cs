using UnityEngine;

[CreateAssetMenu(fileName = "QuestSounds", menuName = "Quests/QuestSounds")]
public class QuestSounds : ScriptableObject
{
    public AudioClip startMission;
    public AudioClip updateMission;
    public AudioClip endMission;
}
