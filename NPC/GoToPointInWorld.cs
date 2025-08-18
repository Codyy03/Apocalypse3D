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
    /// Obróæ npc twarz¹ do gracza
    /// </summary>
    void TurnTowardsPlayer()
    {
        agent.isStopped = true;

        // Oblicz kierunek od NPC do gracza (wektor ró¿nicy pozycji)
        Vector3 direction = (player.position - transform.position).normalized;

        // Ustaw komponent Y na 0, by nie obracaæ NPC w pionie (np. nie patrzy³ w górê/dó³)
        direction.y = 0f;

        // SprawdŸ, czy kierunek nie jest zerowy (czyli gracz nie stoi dok³adnie w tym samym miejscu co NPC)
        if (direction != Vector3.zero)
        {
            // Oblicz rotacjê w kierunku gracza
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // P³ynnie obróæ NPC w stronê gracza z u¿yciem interpolacji sferycznej (Slerp)
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
