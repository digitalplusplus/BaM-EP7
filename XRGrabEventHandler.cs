using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabEventHandler : MonoBehaviour
{
    public ulong avatarNetworkObjectId;
    public GameObject avatarObject;

    public void SelectLeftGrabEnterEventController(SelectEnterEventArgs eventArgs)
    {
        var grabbedObj = eventArgs.interactableObject.transform.GetComponent<NetworkObject >();
        avatarObject.GetComponent<HumanoidNetworkPlayer>().AvatarSelectGrabEnterEventHub(grabbedObj, true);

    }
    public void SelectLeftGrabExitEventController(SelectExitEventArgs eventArgs)
    {
        var grabbedObj = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
        avatarObject.GetComponent<HumanoidNetworkPlayer>().AvatarSelectGrabExitEventHub(grabbedObj, true);

    }
    public void SelectRightGrabEnterEventController(SelectEnterEventArgs eventArgs)
    {
        var grabbedObj = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
        avatarObject.GetComponent<HumanoidNetworkPlayer>().AvatarSelectGrabEnterEventHub(grabbedObj, false);

    }
    public void SelectRightGrabExitEventController(SelectExitEventArgs eventArgs)
    {
        var grabbedObj = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
        avatarObject.GetComponent<HumanoidNetworkPlayer>().AvatarSelectGrabExitEventHub(grabbedObj, false);

    }
}
