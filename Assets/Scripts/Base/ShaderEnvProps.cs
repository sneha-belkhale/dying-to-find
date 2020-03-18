using UnityEngine;

public class ShaderEnvProps : MonoBehaviour
{
    public static ShaderEnvProps instance;

    float lastGlobalPulse;
    [SerializeField] Color globalPulseColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
        Shader.SetGlobalVector("_GlobalPulseColor", globalPulseColor);
    }

    private void Update()
    {
        float globalPulseTimeElapsed = Mathf.Abs(lastGlobalPulse - Time.fixedTime);
        globalPulseTimeElapsed = (globalPulseTimeElapsed > .99f) ? -10f : globalPulseTimeElapsed;
        Shader.SetGlobalFloat("_GlobalPulseTimeElapsed", globalPulseTimeElapsed);
    }

    public void RecordGlobalPulse(Vector3 pos, bool playSound = true)
    {
        if(Time.fixedTime - lastGlobalPulse < 0.2f) return;
        Shader.SetGlobalVector("_GlobalPulseOrigin", pos);
        
        lastGlobalPulse = Time.fixedTime;
        if (playSound)
        {
            UnderWorldLevelCode.instance.PulseSound.Play();
        }
    }

}