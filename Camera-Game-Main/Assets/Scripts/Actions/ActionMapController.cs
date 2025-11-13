using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ActionMapController : MonoBehaviour
{
    [System.Serializable]
    public class InputBinding{
        public string actionName;
        public InputType type;

        [HideInInspector]
        public InputAction action;

        private InputHandler handler;

        public enum InputType{
            Float,
            Vector2
        }

        public void Initialise(InputActionAsset actionSheet, string actionMapName){
            action = actionSheet.FindActionMap(actionMapName).FindAction(actionName);

            switch(type){
                case InputType.Float:
                    handler = new FloatInput(action);
                    break;
                case InputType.Vector2:
                    handler = new Vector2Input(action);
                    break;
            }

            action.Enable();
        }

        public T GetHandler<T>() where T : class, InputHandler{
            return handler as T;
        }

        public Vector2Input AsVector2() => handler as Vector2Input;
        public FloatInput AsFloat() => handler as FloatInput;
    }

    public interface InputHandler { }

    public class Vector2Input : InputHandler{
        private Vector2 valueRaw;
        private Vector2 lastValueRaw;

        public Vector2Input(InputAction action){
            action.performed += context => valueRaw = context.ReadValue<Vector2>();
            action.canceled += context => valueRaw = Vector2.zero;
        }

        public void SetLastValueRaw(Vector2 newValue){
            lastValueRaw = newValue;
        }

        public Vector2 GetValueRaw(){
            return valueRaw;
        }

        public bool ValueChangedUp(){
            if(valueRaw == Vector2.zero && lastValueRaw != Vector2.zero) return true;
            else return false;
        }

        public bool ValueChangedDown(){
            if(valueRaw != Vector2.zero && lastValueRaw == Vector2.zero) return true;
            else return false;
        }


        public bool HasInput(){
            return valueRaw != Vector2.zero;
        }
    }

    public class FloatInput : InputHandler{
        private float valueRaw;
        private float lastValueRaw;

        public FloatInput(InputAction action){
            action.performed += context => valueRaw = context.ReadValue<float>();
            action.canceled += context => valueRaw = 0;
        }

        public void SetLastValueRaw(float newValue){
            lastValueRaw = newValue;
        }

        public float GetValueRaw(){
            return valueRaw;
        }

        public bool ValueChangedUp(){
            if(valueRaw == 0 && lastValueRaw != 0) return true;
            else return false;
        }

        public bool ValueChangedDown(){
            if(valueRaw != 0 && lastValueRaw == 0) return true;
            else return false;
        }

        public bool HasInput(){
            return valueRaw != 0;
        }
    }

    public InputActionAsset actionSheet;

    [SerializeField] private string actionMapName;

    [SerializeField] private List<InputBinding> inputBindings = new List<InputBinding>();

    public static ActionMapController instance;

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        foreach(InputBinding inputBinding in inputBindings){
            inputBinding.Initialise(actionSheet, actionMapName);
        }
    }

    void LateUpdate(){
        foreach(InputBinding inputBinding in inputBindings){
            if(inputBinding.type == InputBinding.InputType.Vector2){
                Vector2Input handler = inputBinding.AsVector2();
                if(handler != null){
                    handler.SetLastValueRaw(handler.GetValueRaw());
                }
            }
            else if(inputBinding.type == InputBinding.InputType.Float){
                FloatInput handler = inputBinding.AsFloat();
                if(handler != null){
                    handler.SetLastValueRaw(handler.GetValueRaw());
                }
            }
        }
    }

    public InputBinding FindInputWithName(string name){
        foreach(InputBinding inputBinding in inputBindings){
            if(inputBinding.actionName.ToLower() == name.ToLower()){
                return inputBinding;
            }
        }
        return null;
    }

}
