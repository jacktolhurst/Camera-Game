using UnityEngine;

public class PhysicalCameraManager : MonoBehaviour, Inspect
{
    [SerializeField] private GameObject playerCameraObject;

    [SerializeField] private float inspectDistance;

    [HideInInspector]
    public bool inspecting { get; private set; } = false;

    private void Update(){
        if(inspecting){
            BringGameObjectToCamera(gameObject, playerCameraObject, inspectDistance);
            RotateGameObjectToCamera(gameObject, playerCameraObject, Vector3.zero);
        }
    }

    public void Inspect(){
        inspecting = true;
    }

    public void CloseInspection(){
        inspecting = false;
        print("CloseInspection");
    }

    private void BringGameObjectToCamera(GameObject obj, GameObject camObj, float distance){
        obj.transform.position = camObj.transform.position + (camObj.transform.forward * distance);
    }

    private void RotateGameObjectToCamera(GameObject obj, GameObject camObj, Vector3 offset){
        obj.transform.eulerAngles = camObj.transform.eulerAngles + offset;
    }
}
