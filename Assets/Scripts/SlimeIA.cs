using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour
{
    private GameManager _GameManager;

    private Animator anim;
    public ParticleSystem hitEffect;
    public int HP;
    private bool isDie;

    public enemyState state;


    //IA
    private bool isWalk;
    private bool isAlert;
    private bool isAttack;
    private bool isPlayerVisible;
    private NavMeshAgent agent;
    private int idWaypoint;
    private Vector3 destination;
    
   

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        //ChangeState(state);
    }

    // Update is called once per frame
    void Update()
    {
        StateManager();

        if(agent.desiredVelocity.magnitude >= 0.1f)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }

        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isAlert", isAlert);
        
    }

    

    IEnumerator Died()
    {
        isDie = true;
        yield return new WaitForSeconds(2.3f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerVisible = true;

            if(state == enemyState.IDLE || state == enemyState.PATROL)
            {
                ChangeState(enemyState.ALERT);
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            isPlayerVisible = false;
        }
    }

    #region MEUS MÉTODOS

    //Método responsável pelo dano / hit
    void GetHit(int amount)
    {
        if(isDie == true) { return; }

        HP -= amount;

        if (HP > 0)
        {
            ChangeState(enemyState.FURY);
            anim.SetTrigger("GetHit");
            hitEffect.Emit(25);
        }
        else
        {
            anim.SetTrigger("Die");
            StartCoroutine("Died");
        }
    }

    void StateManager()
    {
        switch (state)
        {
            case enemyState.ALERT:

                LookAt();

                break;


            case enemyState.FOLLOW:
                // Comportamento quando estiver seguindo
                LookAt();
                destination = _GameManager.player.position;
                agent.destination = destination;

                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }

                break;

            case enemyState.FURY:
                // Comportamento quando estiver em furia 
                LookAt();
                destination = _GameManager.player.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }

                break;

            case enemyState.PATROL:
                // Comportamento quando estiver patrulhando
                break;
        }
    }

    void ChangeState(enemyState newState)
    {
        StopAllCoroutines();// encerra todas as corroutinas
        state = newState;
        isAlert = false;


        switch (state)
        {
            case enemyState.IDLE:

                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;

                StartCoroutine("IDLE");
                
                break;

            case enemyState.ALERT:

                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");

                break;

            case enemyState.PATROL:

                agent.stoppingDistance = 0;
                idWaypoint = Random.Range(0, _GameManager.slimeWayPoints.Length);
                destination = _GameManager.slimeWayPoints[idWaypoint].position;
                agent.destination = destination;

                StartCoroutine("PATROL");

                break;

            case enemyState.FOLLOW:
                isAttack = true;
                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                StartCoroutine("FOLLOW");
                StartCoroutine("ATTACK");

                break;

            case enemyState.FURY:

                destination = transform.position;
                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                agent.destination = destination;

                break;
        }
    }

    IEnumerator IDLE()
    {
        yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);
        StayStill(50); //50% chance de ficar parado ou entrar em patrulha

    }

    IEnumerator PATROL()
    {
        yield return new WaitUntil( () => agent.remainingDistance <= 0);

        StayStill(30); // 30% chance de ficar parado ou 70% para continuar em patrulha
    }

    IEnumerator ALERT()
    {
        yield return new WaitForSeconds(_GameManager.slimeAlertTime);

        if(isPlayerVisible == true)
        {
            ChangeState(enemyState.FOLLOW);
        }
        else
        {
            StayStill(10);
        }
    }

    IEnumerator FOLLOW()
    {
        yield return new WaitUntil(() => !isPlayerVisible);

        print("perdi você");

        yield return new WaitForSeconds(_GameManager.slimeAlertTime);

        StayStill(50);
    }

    IEnumerator ATTACK()
    {
        yield return new WaitForSeconds(_GameManager.slimeAttackDelay);
        isAttack = false;
    }

    void StayStill(int yes)
    {
        if(Rand() <= yes)
        {
            ChangeState(enemyState.IDLE);
        }
        else // Caso NO
        {
            ChangeState(enemyState.PATROL);
        }
    }

    int Rand()
    {
        int rand = Random.Range(0, 100);
        return rand;
    }

    void Attack()
    {
        if(isAttack == false && isPlayerVisible == true)
        {
            isAttack = true;
            anim.SetTrigger("Attack");
        }

        StartCoroutine("ATTACK");
    }

    //void AttackIsDone()
    //{
    //    StartCoroutine("ATTACK");
    //}

    void LookAt()
    {
        Vector3 lookDirection = (_GameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _GameManager.slimeLookAtSpeed * Time.deltaTime);
    }
    #endregion
}
