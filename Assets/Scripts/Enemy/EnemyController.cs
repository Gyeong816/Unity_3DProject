using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyState
{
    void EnterState(EnemyController enemy);
    void UpdateState(EnemyController enemy);
    void ExitState(EnemyController enemy);
}

public enum EnemyType { Enemy1, Enemy2 }




public class EnemyController : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.Enemy1;
    
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.0f;
    public float arrivalThreshold = 0.5f;

    [Header("Detection")]
    public Transform player;
    public PlayerHP playerHP;
    public float detectionRange = 10f;
    public float backDetectionRange = 10f;
    public float viewAngle = 100f;
    public LayerMask obstacleMask;
    public float stopDistance = 5f;

    public bool hasVest = false;
    public bool hasGun = false;
    public bool hasHelmet = false;

    [Header("Combat")]
    public float hp = 100;
    public GameObject vest;
    public GameObject helmet;
    public GameObject enemyAk47;
    public Collider triggerCollider;
    public float shootInterval = 1.0f; 
    public float headHeightOffset = 1.5f;
    
    [Header("Combat")]
    public float waitAfterLostTime = 5f;

    [HideInInspector] public float waitAfterLostTimer = 0f;
    [HideInInspector] public bool reachedLastPosition = false;
    
    [HideInInspector] public int currentIndex = 0;
    [HideInInspector] public float waitTimer;
    [HideInInspector] public float lostSightTimer = 0f;
    [HideInInspector] public Vector3 lastKnownPlayerPosition;
    

    public bool vestSpawned = false;
    public bool gunSpawned = false;
    public bool helmetSpawned = false;
    
    
    public Animator animator;
    public NavMeshAgent nav;
    public EnemyIKHandler enemyIKHandler;

    public bool isDead = false;
    
    public  float forceChaseDuration = 1.5f; 
    
    
    private static readonly int HEADSHOT = Animator.StringToHash("Headshot");
    private static readonly int DIE = Animator.StringToHash("Die");

    private IEnemyState currentState;
    public Transform enemyBody; 

    
    public bool allyHit = false;
    public bool heardGunshot = false;


    private float reducedDamage;
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        triggerCollider.enabled = false;
    }

    private void Start()
    {
        playerHP = player.GetComponent<PlayerHP>();
        currentIndex = Random.Range(0, patrolPoints.Length); 
        ChangeState(new PatrolState());
    }

    private void Update()
    {
        
        currentState?.UpdateState(this);
        DrawView();
    }
    private void LateUpdate()
    {
        SyncBodyTransform();
    }

    private void SyncBodyTransform()
    {
        if (isDead) return; 


        
        enemyBody.position = transform.position;

       
        enemyBody.rotation = transform.rotation * Quaternion.Euler(0, 40f, 0);
    }
    public void ChangeState(IEnemyState newState)
    {
        if (currentState is DeadState) return;

        if (currentState != null && currentState.GetType() == newState.GetType())
            return;
        
        currentState?.ExitState(this);
        currentState = newState;
        
        heardGunshot = false;
        allyHit = false;

        currentState.EnterState(this);
    }

    public void TakeHit(Parts part, float damage, float damageReductionRate)
    {
        if (currentState is DeadState) return;

        switch (part)
        {
            case Parts.Head:
                animator.SetTrigger(HEADSHOT);
                ChangeState(new DeadState());
                break;
            case Parts.Helmet1:
                reducedDamage = damage * (1f - damageReductionRate);
                TakeDamage(reducedDamage);
                Debug.Log($"적 방탄모 보호! 감소된 데미지  {reducedDamage}");
                break;
            case Parts.Vest1:
                reducedDamage = damage * (1f - damageReductionRate);
                TakeDamage(reducedDamage);
                Debug.Log($"적 방탄복 보호! 감소된 데미지  {reducedDamage}");
                break;
            case Parts.Vest3:
                reducedDamage = damage * (1f - damageReductionRate);
                TakeDamage(reducedDamage);
                Debug.Log($"적 방탄복 보호! 감소된 데미지  {reducedDamage}");
                break;
            default:
                TakeDamage(damage);
                Debug.Log($"적 피해  {damage} HP {hp}");
                break;
        }
    }

    private void TakeDamage(float damage)
    {
        
        hp -= damage;
        Debug.Log($"데미지 {damage}, 남은 hp {hp}");
        if (hp <= 0)
        {
            animator.SetTrigger(DIE);
            ChangeState(new DeadState());
        }
        else
        {
            OnHit();
        }
    }

    private void DrawView()
    {
        Vector3 origin = transform.position + Vector3.up * headHeightOffset;

        // 좌우 시야각 방향
        Vector3 leftDir  = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0,  viewAngle / 2, 0) * transform.forward;

        Debug.DrawRay(origin, leftDir  * detectionRange, Color.yellow);
        Debug.DrawRay(origin, rightDir * detectionRange, Color.yellow);
        Debug.DrawRay(origin, transform.forward * detectionRange, Color.red);
    }

    public bool CanSeePlayer()
    {
      
       
        Vector3 origin         = transform.position + Vector3.up * headHeightOffset;
       
        Vector3 targetPos      = player.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer    = (targetPos - origin).normalized;
        float distanceToPlayer = Vector3.Distance(origin, targetPos);


    

   
        if (distanceToPlayer > detectionRange)
            return false;

     
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

      
        int playerLayer = LayerMask.NameToLayer("Player");
        int mask = obstacleMask | (1 << playerLayer);

     
        if (Physics.Raycast(origin, dirToPlayer, out RaycastHit hit, detectionRange, mask, QueryTriggerInteraction.Ignore))
        {
            
            if (hit.collider.gameObject.layer == playerLayer)
                return true;
       
            return false;
        }

        return false;
    }

    
    public bool IsPlayerBehind()
    {
        
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer); 

        float distance = Vector3.Distance(transform.position, player.position);

       
        if (dot < -0.5f && distance < backDetectionRange)
        {
            if (player.TryGetComponent<PlayerController>(out var playerController))
            {
                Debug.Log("뒤에서 플레이어 앉아있어서 감지못함");
                if (playerController.IsCrouching) 
                    return false;
            }
            
            Debug.Log("뒤에서 플레이어 접근 감지!");
            return true;
        }

        return false;
    }

    
    public void OnHit()
    {
        ChangeState(new ChaseState());

    }
    public void AllyHit()
    {
      
      ChangeState(new ChaseState());


    }
    public void HeardGunShot()
    {
        ChangeState(new ChaseState());

    }
    public void LookAtPlayer()
    {
        
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = lookRot; 
        }
    }
    
    public void FireWeapon()
    {
        enemyIKHandler.PickRandomAimTarget(); 

        enemyAk47.GetComponent<EnemyAk47>().Fire();
    }
    

    
    
    
    public void SetHelmetSpawned(bool value) => helmetSpawned = value;
    public void SetVestSpawned(bool value) => vestSpawned = value;
    public void SetGunSpawned(bool value) => gunSpawned = value;
    public void UneqiupVest() => vest.SetActive(false);
    public void UneqiupAk47() => enemyAk47.SetActive(false);

    public void UneqiupHelmet() =>helmet.SetActive(false);

}

