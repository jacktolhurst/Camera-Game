using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{   
    private ActionMapController.Vector2Input moveInput;
    private ActionMapController.FloatInput jumpInput;

    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject cameraPositionObject;

    private Transform playerTrans;
    private Rigidbody playerRb;
    private Bounds playerBounds;

    private Transform cameraTrans;

    private Transform cameraPositionTrans;

    [SerializeField] private LayerMask playerLayerMask;

    [SerializeField] private Vector3 moveDirection;

    [SerializeField] private Vector2 sensitivity;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundSphereRadius;
    private float xRotation; 
    private float yRotation;

    [SerializeField] private string moveActionName;
    [SerializeField] private string jumpActionName;

    void Awake(){
        playerTrans = playerObject.transform;
        playerRb = playerObject.GetComponent<Rigidbody>();
        playerBounds = playerObject.GetComponent<Collider>().bounds;

        cameraTrans = cameraObject.transform;

        cameraPositionTrans = cameraPositionObject.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start(){
        moveInput = ActionMapController.instance.FindInputWithName(moveActionName).AsVector2();
        jumpInput = ActionMapController.instance.FindInputWithName(jumpActionName).AsFloat();
    }

    void FixedUpdate(){ 
        MovePlayer();
    }

    void LateUpdate(){
        MoveCamera();
    }

    private void MoveCamera(){
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yRotation += mouseDelta.x * sensitivity.x; 
        xRotation -= mouseDelta.y * sensitivity.y;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTrans.localRotation = Quaternion.Euler(xRotation, yRotation, 0); 

        // cameraTrans.transform.position = Vector3.Lerp(cameraTrans.transform.position, cameraPositionTrans.position, 10*Time.deltaTime); // Only use this if camera jitters
        cameraTrans.transform.position = cameraPositionTrans.position; // Only use if camera doesnt jitter
    }

    private void MovePlayer(){
        Vector2 moveInputRaw = moveInput.GetValueRaw();
        float jumpInputRaw = jumpInput.GetValueRaw();

        Vector3 moveDir = new Vector3(moveInputRaw.x, 0, moveInputRaw.y);

        Vector3 worldMove = Quaternion.Euler(0, yRotation, 0) * moveDir;

        playerRb.linearVelocity = new Vector3(worldMove.x * moveSpeed, playerRb.linearVelocity.y + (IsGrounded() ? jumpInputRaw * jumpForce : 0),  worldMove.z * moveSpeed);
    }

    private bool IsGrounded(){
        return Physics.CheckSphere(playerTrans.position - new Vector3(0, playerBounds.extents.y, 0), groundSphereRadius, ~playerLayerMask);
    }

    void OnDrawGizmos(){
        if(Application.isPlaying){
            if(IsGrounded()) Gizmos.color = Color.green; 
            else Gizmos.color = Color.red;

            Gizmos.DrawSphere(playerTrans.position - new Vector3(0, playerBounds.extents.y, 0), groundSphereRadius);
        }
    }
}