using UnityEngine;
using extOSC;

public class OSCMouth : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public int Port = 9000;
    public bool debug = false;
    public bool Show_Missing = false;
    private void Start()
    {
        // Create an OSCReceiver object and set the port to listen on
        OSCReceiver receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = Port; // Change this to the desired port number

        // Subscribe to the OSC message event
        receiver.Bind("*", OnOSCMessageReceived);
    }

    private void OnOSCMessageReceived(OSCMessage message)
    {
        //Debug.Log("Recieved: " + message.Address + "   Val:  " + message.Values[0]);
        //Debug.Log("Recieved: " + message.Address + "Val: " + message.Values[0].FloatValue);
        // Check if the message has a string and a float value
        if (message.Values.Count >= 1 && message.Values[0].Type == OSCValueType.Float)
        {
            //string blendshapeName = message.Values[0].StringValue;
            string blendshapeName = message.Address.Substring(1);
            float blendshapeValue = (message.Values[0].FloatValue)*100;

            if (debug)
                Debug.Log("Recieved: " + message.Address + "Val: " + message.Values[0].FloatValue);

            // Set the blendshape value on the skinned mesh renderer
            int blendshapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);
            if (blendshapeIndex >= 0)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, blendshapeValue);
            }
            else
            {
                if(Show_Missing)
                    Debug.LogWarning("Blendshape not found: " + blendshapeName + "  Value: " + blendshapeValue);
            }

          
        }
    }
}
