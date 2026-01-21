using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ParentToSocket : MonoBehaviour
{
    private XRSocketInteractor socket;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void Update()
    {
        if (socket.interactablesSelected.Count > 0)
        {
            Transform attachedObject = socket.interactablesSelected[0].transform;
            if (attachedObject.parent != socket.transform)
            {
                attachedObject.SetParent(socket.transform);
            }
        }
    }
}
