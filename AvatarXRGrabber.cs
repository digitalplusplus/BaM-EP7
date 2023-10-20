using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class AvatarXRGrabber : NetworkBehaviour
{
    InputAction grabCntrlAction, grabHandAction;    //capture actions from our hands/controllers
    bool grabbing; //grabbing state
    NetworkObject grabbedObject;
    Transform attachPoint;
    Toggle thirdPToggle;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        if (transform.name == "Left Arm IK_target")
        {
            grabCntrlAction = GameObject.Find("Camera Offset").transform.Find("Left Controller").GetComponent<ActionBasedController>().selectAction.action;
            grabHandAction = GameObject.Find("Camera Offset").transform.Find("Left Hand").transform.Find("Direct Interactor").GetComponent<ActionBasedController>().activateAction.action;
        }
        if (transform.name == "Right Arm IK_target")
        {
            grabCntrlAction = GameObject.Find("Camera Offset").transform.Find("Right Controller").GetComponent<ActionBasedController>().selectAction.action;
            grabHandAction = GameObject.Find("Camera Offset").transform.Find("Right Hand").transform.Find("Direct Interactor").GetComponent<ActionBasedController>().activateAction.action;
        }

        if ((grabCntrlAction != null) || (grabHandAction != null)) attachPoint = transform.Find("attachPoint");
        thirdPToggle = GameObject.Find("CameraPosition").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (!grabbing) return;      //only update serverRPC if grabbing
        if (!thirdPToggle) return;  //only in 3P mode

        //get trigger/pinch input
        float x = grabCntrlAction.ReadValue<float>() + grabHandAction.ReadValue<float>();
        if (x == 0) //not grabbing
        {
            grabbing = false;
            setIsKinematicServerRpc(grabbedObject, false);  //re-enable IsKinematic
            Debug.Log("3P Release " + grabbedObject.NetworkObjectId);
        }
        else moveMyGrabbedObjectServerRpc(grabbedObject, attachPoint.position, attachPoint.rotation);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;
        if (grabbing) return;           //not grabbing so wait until grab is released 
        if (!thirdPToggle.isOn) return; //FP mode
        
        //get trigger/pinch input
        float x = grabCntrlAction.ReadValue<float>() + grabHandAction.ReadValue<float>();
        if (x>0) //grabbing!
        {
            grabbing = true;
            grabbedObject = other.GetComponent<NetworkObject>();
            setIsKinematicServerRpc(grabbedObject, true);       //grabbing so lets turn on IsKinematic to avoid bouncing objects 
            Debug.Log("3P grabbed " + grabbedObject.NetworkObjectId);
        }
    }

    //============
    //Server Side
    //============
    [ServerRpc] public void moveMyGrabbedObjectServerRpc(NetworkObjectReference grabbedObj, Vector3 position, Quaternion rotation)
    {
        if (!IsServer) return;
        if (grabbedObj.TryGet(out NetworkObject netObj))
        {
            netObj.transform.position = position;
            netObj.transform.rotation = rotation;
            Debug.Log("Client moved object " + netObj.NetworkObjectId + " to position " + netObj.transform.position);
        }
    }

    [ServerRpc] public void setIsKinematicServerRpc(NetworkObjectReference grabbedObj, bool value)
    {
        if (!IsServer) return;
        if (grabbedObj.TryGet(out NetworkObject netObj)){
            netObj.GetComponent<Rigidbody>().isKinematic = value;
        }
    }
}
