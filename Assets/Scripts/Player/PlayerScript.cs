using System;
using System.Data;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : NetworkBehaviour
{
    public UnityEvent<NetworkInputData> AssignInputs;
    Fusion.Addons.Physics.NetworkRigidbody2D rb;

    public Vector2 movementInput;
    public Vector2 lastDirection;
    Vector2 dir;

    public int acceleration;
    int maxSpeed;

    public int player;

    void Awake()
    {
        maxSpeed = acceleration * 2;

        rb = GetBehaviour<Fusion.Addons.Physics.NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        Runner.SetPlayerAlwaysInterested(Object.InputAuthority, Object, true);
        Runner.SetIsSimulated(Object, true);

        if (player == 1)
        {
            AssignInputs.AddListener(AssignInputsP1);
        }
        else if (player == 2)
        {
            AssignInputs.AddListener(AssignInputsP2);
        }
        else
        {
            Debug.LogError($"Player value is undefined. Player value: {player}. If null player is undefined.");
        }
    }

    // CONNECTING CLIENTS CURRENTLY SUFFER FROM JITTERS WHEN MOVING

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            AssignInputs.Invoke(data);
        }

        Movement();
    }

    public void AssignInputsP1(NetworkInputData data)
    {
        dir = data.directionP1;
    }
    
    public void AssignInputsP2(NetworkInputData data)
    {
        dir = data.directionP2;
    }

    public void Movement()
    {
        dir.Normalize();

        if (dir != lastDirection)
        {
            rb.Rigidbody.linearVelocityX = 0;
        }

        if (rb.Rigidbody.linearVelocityX < maxSpeed && rb.Rigidbody.linearVelocityX > -maxSpeed)
        {
            rb.Rigidbody.AddForce(dir * acceleration * 1000 * Runner.DeltaTime, ForceMode2D.Force);
        }

        lastDirection = dir;
    }
}



