using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private Volume volumeComponent;

    private DepthOfField dof;

    private Camera thisCamera;

    [SerializeField] private GameObject dofTargetObject;

    private Dictionary<string, Coroutine> currentInterpolations = new Dictionary<string, Coroutine>();

    [Range(0,1)]
    [SerializeField] private float effectLerpSpeed;

    void Awake(){
        thisCamera = GetComponent<Camera>();
        
        volumeComponent.profile.TryGet<DepthOfField>(out dof);
    }

    void Update(){
        if(dofTargetObject != null) {
            Vector3 vpPos = thisCamera.WorldToViewportPoint(dofTargetObject.transform.position);
            if (vpPos.x >= 0f && vpPos.x <= 1f && vpPos.y >= 0f && vpPos.y <= 1f && vpPos.z > 0f) {
                SetDepthOfField(Vector3.Distance(dofTargetObject.transform.position, transform.position), 300, effectLerpSpeed);
            }
            else{
                SetDepthOfField(0,0,effectLerpSpeed);
            }

        }
    }

    public void SetDepthOfField(float focusDistance, float focusLength, float lerpTime){
        if(currentInterpolations.ContainsKey("focusDistance")) {
            StopCoroutine(currentInterpolations["focusDistance"]);
            currentInterpolations.Remove("focusDistance");
        }
        if(currentInterpolations.ContainsKey("focusLength")) {
            StopCoroutine(currentInterpolations["focusLength"]);
            currentInterpolations.Remove("focusLength");
        }

        currentInterpolations.Add("focusDistance", StartCoroutine(LerpParameter(dof.focusDistance, focusDistance, lerpTime, "focusDistance")));
        currentInterpolations.Add("focusLength", StartCoroutine(LerpParameter(dof.focalLength, focusLength, lerpTime, "focusLength")));
    }

    public IEnumerator LerpParameter<T>(VolumeParameter<T> parameter, T targetValue, float duration, string key){
        float elapsed = 0f;
        T startValue = parameter.value;

        while (elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            parameter.value = LerpValue(startValue, targetValue, t);

            yield return null;
        }

        parameter.value = targetValue;

        if(currentInterpolations.ContainsKey(key)) {
            currentInterpolations.Remove(key);
        }
    }

    private T LerpValue<T>(T start, T end, float t){
        if (typeof(T) == typeof(float))
        return (T)(object)Mathf.Lerp((float)(object)start, (float)(object)end, t);

        if (typeof(T) == typeof(Color))
        return (T)(object)Color.Lerp((Color)(object)start, (Color)(object)end, t);

        if (typeof(T) == typeof(Vector2))
        return (T)(object)Vector2.Lerp((Vector2)(object)start, (Vector2)(object)end, t);

        if (typeof(T) == typeof(Vector3))
        return (T)(object)Vector3.Lerp((Vector3)(object)start, (Vector3)(object)end, t);

        if (typeof(T) == typeof(Vector4))
        return (T)(object)Vector4.Lerp((Vector4)(object)start, (Vector4)(object)end, t);

        return end; 
    }
}