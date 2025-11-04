using UnityEngine;
using System.Collections.Generic;

public class GrabObejcts : MonoBehaviour
{
    private class GrabbedObjectData{
        public GameObject obj = null;
        public Rigidbody rb = null;
        public Transform trans = null;

        public GrabbedObjectData(GameObject newObj){
            if(newObj != null){
                obj = newObj;
                rb = newObj.GetComponent<Rigidbody>();
                trans = newObj.transform;
            }
        }
    }

    private ActionMapController.FloatInput leftClickInput;

    private GrabbedObjectData grabbedObjectData = new GrabbedObjectData(null);

    [SerializeField] private GameObject playerCameraObject;

    private Camera playerCam;

    private Ray lookRay;

    [SerializeField] private LayerMask nonHitLayerMask;

    [SerializeField] private string leftClickActionName;

    private float lastHitTime;
    [SerializeField] private float reloadTime;
    [SerializeField] private float grabDistance;
    [SerializeField] private float grabbedObjectMoveSpeed;
    [SerializeField] private float grabbedObjectThrowSpeed;

    void Awake(){
        playerCam = playerCameraObject.GetComponent<Camera>();
    }

    void Start(){
        leftClickInput = ActionMapController.instance.FindInputWithName(leftClickActionName).AsFloat();
    }

    void Update(){
        if(leftClickInput.GetValueRaw() != 0 && Time.time - reloadTime > lastHitTime){
            lastHitTime = Time.time;

            grabbedObjectData = GetGrabbedObjectData(grabbedObjectData);
        }
    }

    void FixedUpdate(){
        if(grabbedObjectData.obj != null) {
            lookRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
            MoveGrabbedObject();
        }
    }

    private void MoveGrabbedObject(){
        Vector3 targetPosition = lookRay.origin + lookRay.direction * grabDistance;

        Vector3 direction = (targetPosition - grabbedObjectData.trans.position).normalized;
        float distance = Vector3.Distance(targetPosition, grabbedObjectData.trans.position);
        
        grabbedObjectData.rb.linearVelocity = direction * grabbedObjectMoveSpeed * distance;
    }

    private GrabbedObjectData GetGrabbedObjectData(GrabbedObjectData objectData){
        if(objectData.obj == null){
            GameObject targetedObject = GetTargetedObject(grabDistance);
            if(targetedObject != null){
                return new GrabbedObjectData(targetedObject);
            }
        } 
        else{
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
