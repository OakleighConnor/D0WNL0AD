using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : NetworkBehaviour
{
    public Vector2 movementInput;

    public UnityEvent movement;
    Rigidbody2D rb;

    public int speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if(HasStateAuthority.Input)
        movement.Invoke();
    }

    public void MovementP1()
    {
        Debug.Log("Player 1 movement");
        if(GetInput(out NetworkInputData data))
        {
            data.directionP1.Normalize();

            rb.AddForce(data.directionP1 * speed * 10);
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



