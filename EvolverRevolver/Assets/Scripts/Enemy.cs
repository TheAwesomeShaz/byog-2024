using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public event Action OnDeath;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private bool playerDetected;
    [SerializeField] private int health = 100;
    [SerializeField] private float enemyWaitAtPointTime;

    [Header("Animation Related")]
    [SerializeField] private float animationTransitionTime = 0.07f;
    [SerializeField] private string idleAnimString;
    [SerializeField] private string walkingAnimString;
    [SerializeField] private string ChasingAnimString;
    [SerializeField] private string confusedAnimString;
    [SerializeField] private string shootingAnimString;


    private Transform[] patrolPoints;
    private EnemyState currentState;
    private int currentPatrolIndex;

    private void Start()
    {
        currentState = EnemyState.Idle;
        StartCoroutine(Patrolling());
    }

    private IEnumerator Patrolling()
    {
        while (!playerDetected) 
        {
            if (patrolPoints.Length == 0)
            {
                ChangeState(EnemyState.Idle);
                Debug.Log("No Patrol Points Assigned");
                yield break;
            }

            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            ChangeState(EnemyState.Patrolling);
            
            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                // wait until enemy is close to a patrol point
                yield return null; 
            }

            ChangeState (EnemyState.Idle);
            yield return new WaitForSeconds(enemyWaitAtPointTime);

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

    }

    public void SetPatrolPoints(Transform[] patrolPoints)
    {
        this.patrolPoints = patrolPoints;
    }

    private void ChangeState(EnemyState state)
    {
        if (currentState != state)
        {
            currentState = state;
            switch (currentState)
            {
                case EnemyState.Idle:
                    animator.CrossFade(idleAnimString, animationTransitionTime);
                    break;
                case EnemyState.Patrolling:
                    animator.CrossFade(walkingAnimString, animationTransitionTime);

                    break;
                case EnemyState.ChasingPlayer:
                    animator.CrossFade(ChasingAnimString, animationTransitionTime);
                    break;
                case EnemyState.Shooting:
                    animator.CrossFade(shootingAnimString, animationTransitionTime);
                    break;
                case EnemyState.Confused:
                    animator.CrossFade(confusedAnimString, animationTransitionTime);
                    break;
                case EnemyState.WalkingBackToPatrol:
                    animator.CrossFade(walkingAnimString, animationTransitionTime);
                    break;
                default:
                    Debug.Log("Enemy Change State Called with the wrong state");
                    break;
            }
        }
    }

}

public enum EnemyState
{
    Idle,
    Patrolling,
    ChasingPlayer,
    Shooting,
    Confused,
    WalkingBackToPatrol,
}
