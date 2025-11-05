using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GrabObejcts : MonoBehaviour
{
    private class GrabbedObjectData{
        public GameObject obj = null;
        public Rigidbody rb = null;
        public Transform trans = null;
        public Inspect inspectScript = null;

        public GrabbedObjectData(GameObject newObj, List<Collider> playerColliders=null){
            if(newObj != null){
                obj = newObj;

                if(playerColliders != null){ 
                    foreach (Collider collider in obj.GetComponents<Collider>()){
                        foreach(Collider playerCollider in playerColliders){
                            Physics.IgnoreCollision(collider, playerCollider, true);
                        }
                    }
                }

                rb = newObj.GetComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.useGravity = false;

                
                trans = newObj.transform;

                inspectScript = newObj.GetComponent<Inspect>();
            }
        }

        public void Delete(List<Collider> playerColliders){
            foreach (Collider collider in obj.GetComponents<Collider>()){
                foreach(Collider playerCollider in playerColliders){
                    Physics.IgnoreCollision(collider, playerCollider, false);
                }
            }

            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.useGravity = true;
        }
    }

    private ActionMapController.Vector2Input rotateInput;
    private ActionMapController.FloatInput leftClickInput;
    private ActionMapController.FloatInput pullInput;
    private ActionMapController.FloatInput inspectInput;

    private GrabbedObjectData grabbedObjectData = new GrabbedObjectData(null);

    [SerializeField] private GameObject playerCameraObject;
    [SerializeField] private GameObject playerColliderObject;

    private List<Collider> playerColliders = new List<Collider>();

    private Camera playerCam;

    private Ray lookRay;

    [SerializeField] private LayerMask nonHitLayerMask;

    [SerializeField] private string rotateActionName;
    [SerializeField] private string leftClickActionName;
    [SerializeField] private string pullActionName;
    [SerializeField] private string inspectActionName;

    private float lastHitTime;
    [SerializeField] private float reloadTime;
    private float grabDistance;
    [SerializeField] private float minGrabDistance;
    [SerializeField] private float maxGrabDistance;
    [SerializeField] private float grabbedObjectMoveSpeed;
    [SerializeField] private float grabbedObjectThrowSpeed;
    [SerializeField] private float grabbedObjectRotateSpeed;

    void Awake(){
        playerColliders = playerColliderObject.GetComponents<Collider>().ToList();

        playerCam = playerCameraObject.GetComponent<Camera>();

        grabDistance = (minGrabDistance + maxGrabDistance)/2;
    }

    void Start(){
        rotateInput = ActionMapController.instance.FindInputWithName(rotateActionName).AsVector2();
        leftClickInput = ActionMapController.instance.FindInputWithName(leftClickActionName).AsFloat();
        pullInput = ActionMapController.instance.FindInputWithName(pullActionName).AsFloat();
        inspectInput = ActionMapController.instance.FindInputWithName(inspectActionName).AsFloat();
    }

    void Update(){
        if(Time.time - reloadTime > lastHitTime && leftClickInput.GetValueRaw() != 0){
            lastHitTime = Time.time;

            grabbedObjectData = GetGrabbedObjectData(grabbedObjectData);
        }

        if(grabbedObjectData.obj != null){
            grabDistance = Mathf.Clamp(grabDistance + pullInput.GetValueRaw(), minGrabDistance, maxGrabDistance);

            if(inspectInput.GetValueRaw() != 0){
                if(grabbedObjectData.inspectScript != null ){
                    if(!grabbedObjectData.inspectScript.inspecting) grabbedObjectData.inspectScript.Inspect();
                    else grabbedObjectData.inspectScript.CloseInspection();
                }
                else{
                    grabDistance = minGrabDistance;
                }
            }
        }

        lookRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
    }

    void FixedUpdate(){
        if(grabbedObjectData.obj != null) {
            MoveGrabbedObject();
        }
    }

    private void MoveGrabbedObject(){
        Vector3 targetPosition = lookRay.origin + lookRay.direction * grabDistance;
        Vector3 direction = (targetPosition - grabbedObjectData.rb.position).normalized;
        float distance = Vector3.Distance(targetPosition, grabbedObjectData.rb.position);
        
        grabbedObjectData.rb.linearVelocity = direction * grabbedObjectMoveSpeed * distance;

        Vector2 rotateInputRaw = rotateInput.GetValueRaw();
        Vector3 rotateDir = new Vector3(rotateInputRaw.y, -rotateInputRaw.x, 0f); 
        grabbedObjectData.rb.angularVelocity = Vector3.ClampMagnitude(rotateDir * grabbedObjectRotateSpeed, grabbedObjectData.rb.maxAngularVelocity);
    }

    private GrabbedObjectData GetGrabbedObjectData(GrabbedObjectData objectData){
        if(objectData.obj == null){
            GameObject targetedObject = GetTargetedObject(5);
            if(targetedObject != null){
                return new GrabbedObjectData(targetedObject, playerColliders);
            }
        } 
        else{
            grabbedObjectData.inspectScript.CloseInspection();
            objectData.Delete(playerColliders);
            objectData.rb.linearVelocity = objectData.rb.linearVelocity + lookRay.direction * grabbedObjectThrowSpeed;
        }

        return new GrabbedObjectData(null);
    }

    private GameObject GetTargetedObject(float rayDistance){
        Ray mainRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if(Physics.Raycast(mainRay, out RaycastHit hit, rayDistance, ~nonHitLayerMask, QueryTriggerInteraction.Ignore)){
            if(hit.collider.GetComponent<Rigidbody>()){
                return hit.collider.gameObject;
            }
        }
        return null;
    }
}
