using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchBehaviour : MonoBehaviour
{
    void Start()
    {
        Light2D l = gameObject.GetComponent<Light2D>();
        if (l == null) Debug.LogError("Trying to add torch light with no Light2D component");
        else GameManager.instance.AddTorchGM(l);
    }
}
