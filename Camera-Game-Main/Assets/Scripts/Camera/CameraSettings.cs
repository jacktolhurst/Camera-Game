using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private Volume volumeComponent;

    private DepthOfField dof;

    [SerializeField] private GameObject dofTargetObject;

    [SerializeField] private float length;

    void Start(){
        volumeComponent.profile.TryGet<DepthOfField>(out dof);
    }

    void Update(){

        dof.focusDistance.value = Vector3.Distance(dofTargetObject.transform.position, transform.position); 
        dof.focalLength.value = length; 
    }
}