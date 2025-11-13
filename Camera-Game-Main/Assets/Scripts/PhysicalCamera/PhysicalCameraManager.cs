using UnityEngine;

public class PhysicalCameraManager : MonoBehaviour, Inspect
{
    [SerializeField] private GameObject playerCameraObject;
    private Rigidbody physicalCameraRb;

    private Quaternion targetRot;

    private Vector3 targetPos;

    [SerializeField] private float inspectDistance;
    [SerializeField] private float inspectMoveSpeed;
    [SerializeField] private float inspectRotateSpeed;
    
    [HideInInspector]
    public bool inspecting { get; private set; } = false;
    
    void Start(){
        physicalCameraRb = GetComponent<Rigidbody>();
    }
    
    void Update(){
        if(inspecting){
            targetPos = GameObjectToCameraPosition(playerCameraObject, inspectDistance);
            targetRot = playerCameraObject.transform.rotation;
        }
    }
    
    void FixedUpdate(){
        if(inspecting){
            Vector3 direction = (targetPos - physicalCameraRb.position).normalized;
            float distance = Vector3.Distance(physicalCameraRb.position, targetPos);

            physicalCameraRb.linearVelocity = direction * inspectMoveSpeed * distance;
            // physicalCameraRb.MovePosition(targetPos);

            Quaternion newRot = Quaternion.Slerp(physicalCameraRb.rotation, targetRot, inspectRotateSpeed);
            physicalCameraRb.MoveRotation(newRot);
        }
    }
    
    public void Inspect(){
        inspecting = true;
    }
    
    public void CloseInspection(){
        inspecting = false;
        print("CloseInspection");
    }
    
    private Vector3 GameObjectToCameraPosition(GameObject camObj, float distance){
        return camObj.transform.position + (camObj.transform.forward * distance);
    }
}