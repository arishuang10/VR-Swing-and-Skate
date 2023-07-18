using UnityEngine;

public class MyScript : MonoBehaviour
{
    // Declare public variables here that can be set in the Inspector
    public int myInt = 10;
    public float myFloat = 1.5f;
    public string myString = "Hello, world!";

    // Declare private variables here
    private bool myBool = true;
    private Transform myTransform;

    // This function is called once when the script is first loaded
    void Awake()
    {
        // Do any initialization here
        myTransform = GetComponent<Transform>();
    }

    // This function is called every frame
    void Update()
    {
        // Do any updates here
        myTransform.position += Vector3.up * Time.deltaTime * myFloat;
    }

    // Declare public functions here that can be called from other scripts
    public void MyPublicFunction()
    {
        // Do something here
    }

    // Declare private functions here that are only used within this script
    private void MyPrivateFunction()
    {
        // Do something here
    }
}
