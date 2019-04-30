using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITankController : FSM
{
    public TankShooting tankShooter;
    public TankHealth tankHealth;
    private bool isDead = false;
    private float elapsedTime = 0.0f;
    private float shootRate = 3.0f;
    private GameObject player = null;
    private NavMeshAgent navMeshAgent;
    public enum FSMState
    {
        None, Patrol, Attack, Dead,
    }
    //Current state that the NPC is reaching
    public FSMState curState;

    protected override void Initialize()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Get the list of patrol points
        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");
        Debug.Log(pointList.Length);

        int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
        destPos = pointList[rndIndex].transform.position;
    }

    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }
        elapsedTime += Time.deltaTime;

        // Go to dead state if there is no health left
        if (this.tankHealth.m_CurrentHealth <= 0)
            curState = FSMState.Dead;
    }

    private void UpdateDeadState()
    {
        if (!isDead)
        {
            Debug.Log("Dead");
        }
    }

    private void UpdateAttackState()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask(new string[] { "Players", "Bots"}));
        //Collider[] bots = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask("Bots"));

        // editing this because I'm getting bots and players
        if (players.Length == 1)
        {
            curState = FSMState.Patrol;
            player = null;
            navMeshAgent.enabled = true;
            return;
        }

        // gets the gameobject that isn't itself, since it's inside the same array
        foreach (Collider play in players)
        {
            if (this.gameObject != play.gameObject) player = play.gameObject;
        }

        Vector3 _direction = (player.transform.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 3);
        if (elapsedTime > shootRate)
        {
            this.tankShooter.Fire();
            elapsedTime = 0;
        }
    }

    private void UpdatePatrolState()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 10.0f, LayerMask.GetMask(new string[] { "Players", "Bots" }));
        if (players.Length > 1)
        {
            curState = FSMState.Attack;

            foreach (Collider play in players)
            {
                if (this.gameObject != play.gameObject) player = play.gameObject;
            }

            navMeshAgent.enabled = false;
            return;
        }
        if (IsInCurrentRange(destPos))
        {
            int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
            destPos = pointList[rndIndex].transform.position;
        }
        navMeshAgent.destination = destPos;
    }

    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);
        if (xPos <= 5 && zPos <= 5) return true;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }
}
