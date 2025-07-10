using UnityEngine.XR;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public EInput inputType;

    public MenuManager menuManager;

    //parent of transform (to apply planet operations)
    public Transform cameraElbow;

    public Quaternion frame = Quaternion.identity;

    //if there is no specified focus, this one will be applied
    public Transform defaultFocus;

    //transform to follow around
    public Transform focus;

    //values of the current camera rotation around the focus
    public float yaw = 0.0f;
    public float pitch = 0.0f;

    public float range = 5.0f;
    public float trueRange = 5.0f;

    //allow the editor to change this value within reason
    public float sensitivity = 0.0f;

    public KeyCode connect = KeyCode.JoystickButton0;
    public KeyCode release = KeyCode.JoystickButton6;

    public TouchData touchData;

    private float collisionRadius = 0.2f;
    public LayerMask blockingMask;

    public bool isThirdPerson = false;
    public bool fuzz = false;
    public bool isRouted = false;

    private Quaternion prevHeadsetRotation;

    void Start() // <-- why did you put a space between Start and ()??? >:(
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    public void Poll()
    {
        if (menuManager != null)
        {
            //confine the mouse to the middle of the screen
            if (Input.GetKeyDown(connect))
            {
                if (!menuManager.isActive)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            //unlock the mouse from the middle of the screen
            if (Input.GetKeyDown(release))
            {
                if (!menuManager.isActive)
                {
                    Cursor.lockState = CursorLockMode.None;
                    menuManager.ActivateMenu();
                }
            }
        }

        if (isRouted)
        {
            if (!isThirdPerson)
            {
                TogglePerspective();
            }

            range = 8.0f;
        }
        else
        {
            range = trueRange;
        }

        if (inputType == EInput.VIRTUAL) // for some reason when you turn 180 y going up/down not work or something idk (((still broken)))
        {
            
            Quaternion headsetRotation = InputTracking.GetLocalRotation(XRNode.Head);
            
            // headsetRotation.z = 0f;

            float prevYaw = prevHeadsetRotation.eulerAngles.y * Mathf.Deg2Rad;
            float currYaw = headsetRotation.eulerAngles.y * Mathf.Deg2Rad;
            
            float diffYaw = currYaw - prevYaw;
            
            yaw += diffYaw;
            pitch = -headsetRotation.eulerAngles.x * Mathf.Deg2Rad;
			         
            transform.localRotation = headsetRotation; // does not pull from any source other than headsetRotation
           
            prevHeadsetRotation = headsetRotation;
        }
        if (fuzz)
        {
            yaw = Random.Range(-Mathf.PI, Mathf.PI);
            pitch = Random.Range(-Mathf.PI * 0.5f + 0.001f, Mathf.PI * 0.5f - 0.001f);
        }

        if (focus == null)
        {
            focus = defaultFocus;
        }

        cameraElbow.rotation = frame;

        Vector3 lf = MathExtension.DirectionFromYawPitch(yaw, pitch);
        Vector3 f = cameraElbow.TransformDirection(lf);

        RaycastHit collInfo;

        Vector3 target = focus.position + f * range;
        
        if (Physics.SphereCast(focus.position, collisionRadius, (target - focus.position).normalized, out collInfo, range, blockingMask.value))
        {
            cameraElbow.position = collInfo.point + collInfo.normal * collisionRadius;
        }
        else
        {
            cameraElbow.position = focus.position + f * range;
        }
        
        transform.localRotation = Quaternion.LookRotation(-lf);
    }

    public void TogglePerspective() {}

    public void CameraCorrection(Quaternion oldFrame, Quaternion newFrame) // issue?
    {
        Vector3 of = oldFrame * Vector3.forward;
        Vector3 nf = newFrame * Vector3.forward;

        Vector2 of2 = new Vector2(of.x, of.z);
        Vector2 nf2 = new Vector2(nf.x, nf.z);

        float yawCorrection = Vector2.SignedAngle(of2.normalized, nf2.normalized) * Mathf.Deg2Rad;

        yaw += yawCorrection;
    }

    public void SetCamera(float yaw, float pitch)
    {
        // this.yaw = yaw;
        // this.pitch = pitch;
    }
}
