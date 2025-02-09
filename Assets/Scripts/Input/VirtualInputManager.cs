using System;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.InputSystem;

//wrapper class for input sample, essentially allows the client to assume control over their copy
public class VirtualInputManager : InputManager
{
    public VirtualInputSample sample;

    public bool hasFocus = false;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Start() {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    public override void Initialise()
    {
        sample = new VirtualInputSample(); 
        sample.Initialise();

        cameraController.inputType = EInput.VIRTUAL;
        cameraController.menuManager.inputType = EInput.VIRTUAL;
    }

    public override InputSample GetInputSample()
    {
        return sample;
    }

    public override void PerFrameUpdate()
    {
        //inputs must be sampled every frams
        // getting the keys pressed
        
        float XInput = moveAction.ReadValue<Vector2>().x;
        float YInput = moveAction.ReadValue<Vector2>().y;
        bool jump;

        if (jumpAction.WasPressedThisFrame()) {
            jump = true;
        }
        else
        {
            jump = false;
        }
        
        
        sample.left.Poll(menuOverride, (XInput < -0.2f), fuzz);
        sample.right.Poll(menuOverride, (XInput > 0.2f), fuzz);
        sample.forward.Poll(menuOverride, (YInput > 0.2f), fuzz);
        sample.backward.Poll(menuOverride, (YInput < -0.2f), fuzz);
        sample.jump.Poll(menuOverride, jump,fuzz);
        
        cameraController.Poll();

        bool hasFocusCurrent = Cursor.lockState == CursorLockMode.Locked;

        //require the fire button to be released before it can be triggered
        if (hasFocus != hasFocusCurrent)
        {
            sample.fire.requireRelease = true;
        }

        hasFocus = hasFocusCurrent;

        sample.fire.Poll(menuOverride, fuzz);

        sample.yaw = cameraController.yaw;

        sample.pitch = 0.0f;

        if (!cameraController.isThirdPerson || cameraController.isRouted)
        {
            sample.pitch = cameraController.pitch;
        }
    }

    public override void Tick()
    {
        sample.left.Reset();
        sample.right.Reset();
        sample.forward.Reset();
        sample.backward.Reset();
        sample.jump.Reset();
        sample.fire.Reset();

        sample.timestamp++;

        if (sample.timestamp >= Settings.MAX_INPUT_INDEX)
        {
            sample.timestamp -= Settings.MAX_INPUT_INDEX;
        }
    }
}
