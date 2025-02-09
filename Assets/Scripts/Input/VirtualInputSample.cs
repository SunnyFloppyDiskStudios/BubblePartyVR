using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualInputSample : InputSample
{
    public Button left;
    public Button right;
    public Button forward;
    public Button backward;

    private InputAction moveAction;

    public override void Initialise()
    {
        type = EInput.VIRTUAL;

        base.Initialise();

        left = new Button(KeyCode.A);
        right = new Button(KeyCode.D);
        forward = new Button(KeyCode.W);
        backward = new Button(KeyCode.S);

        moveAction = InputSystem.actions.FindAction("Move");
    }
    

    public override Vector2 GetMovementVector()
    {
        // movement code --
        Vector2 vector = Vector2.zero;
        
        if (Input.GetAxis("Horizontal") < 0.2f)
        {
            vector.x += 1.0f;
        }
        
        if (Input.GetAxis("Horizontal") > 0.2f)
        {
            vector.x += -1.0f;
        }
        
        if (Input.GetAxis("Vertical") > 0.2f)
        {
            vector.y += 1.0f;
        }
        
        if (Input.GetAxis("Vertical") < 0.2f)
        {
            vector.y += -1.0f;
        }
        // --

        //normalise vector that's diagonal
        if (vector.x != 0.0f && vector.y != 0.0f)
        {
            vector.Normalize();
        }

        return vector;
    }

    public override Vector3 GetLookVector()
    {
        return MathExtension.DirectionFromYawPitch(yaw, pitch);
    }

    //entropy encoding: 0 for no left / right, 1 for left or right followed by the identifier bit
    public override void WriteToStream(ref BitStream stream)
    {
        base.WriteToStream(ref stream);

        stream.WriteInt((int)left.state, 2);
        stream.WriteInt((int)right.state, 2);
        stream.WriteInt((int)forward.state, 2);
        stream.WriteInt((int)backward.state, 2);
    }

    public override void ReadFromStream(ref BitStream stream)
    {
        base.ReadFromStream(ref stream);

        byte leftBit = stream.ReadBits(2)[0];
        byte rightBit = stream.ReadBits(2)[0];
        byte forwardBit = stream.ReadBits(2)[0];
        byte backwardBit = stream.ReadBits(2)[0];
        
        left.state = (EButtonState)leftBit;
        right.state = (EButtonState)rightBit;
        forward.state = (EButtonState)forwardBit;
        backward.state = (EButtonState)backwardBit;
    }

    public override InputSample Clone()
    {
        VirtualInputSample copy = new VirtualInputSample();

        copy.timestamp = timestamp;

        copy.jump = jump.Clone();
        copy.fire = fire.Clone();

        copy.yaw = yaw;
        copy.pitch = pitch;

        copy.left = left.Clone();
        copy.right = right.Clone();
        copy.forward = forward.Clone();
        copy.backward = backward.Clone();

        return copy;
    }

    public override void Print()
    {
        Debug.Log("Left: " + left.state.ToString());
        Debug.Log("Right: " + right.state.ToString());
        Debug.Log("Forward: " + forward.state.ToString());
        Debug.Log("Backward: " + backward.state.ToString());
        Debug.Log("Jump: " + jump.state.ToString());
        Debug.Log("Yaw: " + yaw.ToString());
        Debug.Log("Pitch: " + pitch.ToString());
    }
}
