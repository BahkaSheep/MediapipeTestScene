using UnityEngine;
using extOSC;
using System.Collections.Generic;

public class OSCRecieverBlendShapes : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public int Port = 9000;
    public bool debug = false;
    public AnimationCurve BlinkCurve;
    public AnimationCurve EverythingCurve;
    public float lerpSpeed = 10f;

    string blendshapeName;
    float blendshapeValue;
    int blendshapeIndex;

    Dictionary<string, float> currentValues = new Dictionary<string, float>();
    Dictionary<string, float> targetValues = new Dictionary<string, float>();

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
        //Debug.Log("Recieved: " + message.Address + "Val: " + message.Values[0]);
        // Check if the message has a string and a float value
        if (message.Values.Count >= 2 && message.Values[0].Type == OSCValueType.String && message.Values[1].Type == OSCValueType.Float)
        {
            //string blendshapeName = message.Values[0].StringValue;
            blendshapeName = message.Values[0].StringValue;
            blendshapeValue = message.Values[1].FloatValue;


            // Set the blendshape value on the skinned mesh renderer
            blendshapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);
            if (blendshapeIndex >= 0)
            {

                if (targetValues.ContainsKey(blendshapeName))
                {
                    targetValues[blendshapeName] = blendshapeValue;

                }
                else
                    targetValues.Add(blendshapeName, blendshapeValue);

            }
            else
            {
                Debug.LogWarning("Blendshape not found: " + blendshapeName);
            }

            if (debug)
                Debug.LogWarning("Recieved: " + blendshapeName + "Val: " + blendshapeValue);
        }
    }

    
    public void FixedUpdate()

    {

        foreach (KeyValuePair<string, float> keyValuePairs in targetValues)
        {
            if (currentValues.ContainsKey(keyValuePairs.Key))
            { 
                currentValues[keyValuePairs.Key] = Mathf.Lerp(currentValues[keyValuePairs.Key], keyValuePairs.Value, Time.deltaTime* lerpSpeed);
                UpdateBlendshapes(keyValuePairs.Key, currentValues[keyValuePairs.Key]);

            }
            else
            {
                currentValues.Add(keyValuePairs.Key, 0);                
            }

            

        }

    }
    






    void UpdateBlendshapes(string blendshapeName, float blendshapeValue) {


        blendshapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);

        switch (blendshapeName)
        {


            case "eyeBlinkLeft":
                {
                    float multiplier = BlinkCurve.Evaluate(blendshapeValue / 100);
                    skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, multiplier * 100);
                }
                break;

            case "eyeBlinkRight":
                {
                    float multiplier = BlinkCurve.Evaluate(blendshapeValue / 100);
                    skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, multiplier * 100);
                }
                break;

            default:
                {
                    float multiplier = EverythingCurve.Evaluate(blendshapeValue / 100);
                    skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, multiplier * 100);
                }
                break;
        }
    }
}




