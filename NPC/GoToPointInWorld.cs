using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BasicMotionAnimations))]
public class GoToPointInWorld : MonoBehaviour
{
    [SerializeField] Transform destinationPoint;
    [SerializeField] float stopingDistance = 0.5f;

    [SerializeField] Transform player;

    [SerializeField] string achiveTargetAnimation;
    bool targetIsAchive;

    NavMeshAgent agent;
    BasicMotionAnimations motionAnimations;

    public bool goToPoint = false;
    private void Awake()
    {
        motionAnimations = GetComponent<BasicMotionAnimations>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.SetDestination(destinationPoint.position);
        agent.isStopped = true;
    }
    void Update()
    {
        if (!goToPoint)
        {
            HandleIdle();
            return;
        }

        if(Dialogues.DialoguesManager.dialogueIsActive)
        {
            TurnTowardsPlayer();
            return;
        }
        HandleWalking();
    }
    /// <summary>
    /// Obr�� npc twarz� do gracza
    /// </summary>
    void TurnTowardsPlayer()
    {
        agent.isStopped = true;

        // Oblicz kierunek od NPC do gracza (wektor r�nicy pozycji)
        Vector3 direction = (player.position - transform.position).normalized;

        // Ustaw komponent Y na 0, by nie obraca� NPC w pionie (np. nie patrzy� w g�r�/d�)
        direction.y = 0f;

        // Sprawd�, czy kierunek nie jest zerowy (czyli gracz nie stoi dok�adnie w tym samym miejscu co NPC)
        if (direction != Vector3.zero)
        {
            // Oblicz rotacj� w kierunku gracza
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // P�ynnie obr�� NPC w stron� gracza z u�yciem interpolacji sferycznej (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        motionAnimations.ChangeAnimation(motionAnimations.idle);
    }
    void HandleWalking()
    {
        agent.isStopped = false;

        if (agent.remainingDistance >= stopingDistance)
        {
            motionAnimations.ChangeAnimation(motionAnimations.walk);
        }
        else
        {
            if (!string.IsNullOrEmpty(achiveTargetAnimation))
                motionAnimations.ChangeAnimation(achiveTargetAnimation);

            targetIsAchive = true;
            agent.ResetPath();
            goToPoint = false;
        }
    }

    void HandleIdle()
    {
        agent.isStopped = true;

        if (!targetIsAchive)
            motionAnimations.ChangeAnimation(motionAnimations.idle);
    }
    public void SetGoToPoint(bool value) => goToPoint = value;

}
