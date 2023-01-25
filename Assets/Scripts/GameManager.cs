using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    IDLE, ALERT, PATROL, FOLLOW, FURY
}

public class GameManager : MonoBehaviour
{

    public Transform player;

    [Header("Slime IA")]
    public float slimeIdleWaitTime;
    public Transform[] slimeWayPoints;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
