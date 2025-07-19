using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialQuest : Quest
{
    QuestManager questManager;

    [SerializeField] List<EnemyHealth> enemies;

    [SerializeField] int deadEnemiesCounter;

    [SerializeField] GameObject npc,eatingZombie;
    [SerializeField] AudioClip screamSound;
    private void Awake()
    {
        questManager = FindFirstObjectByType<QuestManager>();
        enemies = FindObjectsByType<EnemyHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public override void StartQuest()
    {
        questStage = 1;
        questManager.AddQuest(this);
        state = QuestState.Active;
        questPorgress = stages[0];
        audioManager.PlayClip(startMission);
    }

    public override void EndQuest()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateQuest()
    {
        questStage++;
        audioManager.PlayClip(updateMission);

        switch (questStage)
        {
            case 2: questPorgress = stages[1]; npc.GetComponent<Animator>().SetTrigger("Death");
                eatingZombie.SetActive(true); npc.transform.position = new Vector3(-41.607f, 5.14f, 87.63f);
                audioManager.PlayClip(screamSound);
                break;

            case 3:
                questPorgress = stages[2]; break;

        }
        questManager.UpdateQuest();
    }

    public void IncreseDeadEnemiesCounter()
    {
        deadEnemiesCounter++;

        questPorgress = $"Zabij zombie {deadEnemiesCounter}/3";
        questManager.UpdateQuest();
        
        if(deadEnemiesCounter == 3)
            UpdateQuest(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (questStage != 2) return;

        if (other.CompareTag("Player"))
        {
            UpdateQuest();
        }
    }
}
