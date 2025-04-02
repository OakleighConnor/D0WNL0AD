using System;
using System.Data;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : NetworkBehaviour
{
    public Vector2 movementInput;

    public UnityEvent movement;
    Fusion.Addons.Physics.NetworkRigidbody2D rb;

    public Vector2 lastDirection;

    public int acceleration;
    int maxSpeed;

    void Awake()
    {
        maxSpeed = acceleration * 2;

        rb = GetBehaviour<Fusion.Addons.Physics.NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        Runner.SetPlayerAlwaysInterested(Object.InputAuthority, Object, true);
        Runner.SetIsSimulated(Object, true);
    }

    // CONNECTING CLIENTS CURRENTLY SUFFER FROM JITTERS WHEN MOVING

    public override void FixedUpdateNetwork()
    {
        movement.Invoke();
    }

    public void MovementP1()
    {
        Debug.Log("Player 1 movement");
        if(GetInput(out NetworkInputData data))
        {
            data.directionP1.Normalize();

            if(data.directionP1 != lastDirection)
            {
                rb.Rigidbody.linearVelocityX = 0;
            }

            if(rb.Rigidbody.linearVelocityX < maxSpeed && rb.Rigidbody.linearVelocityX > -maxSpeed)
            {
                rb.Rigidbody.AddForce(data.directionP1 * acceleration * 1000 * Runner.DeltaTime, ForceMode2D.Force);
            }

            lastDirection = data.directionP1;
        }
    }
    
    public void MovementP2()
    {
        Debug.Log("Player 2 movement");
        if(GetInput(out NetworkInputData data))
        {
            /*
            data.directionP2.Normalize();
            cc.Move(5 * data.directionP2 * Runner.DeltaTime);
            anim.SetInteger("Direction", Mathf.RoundToInt(data.directionP1.x));
            */
        }
    }
}



