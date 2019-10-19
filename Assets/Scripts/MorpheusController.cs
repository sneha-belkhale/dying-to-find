using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorpheusController : MonoBehaviour
{
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
      mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
      StartCoroutine(Glitch());
    }

    IEnumerator Glitch()
    {
        while(true){
          float glitchDuration = Random.Range(1f,2f);
          float glitchSpeed = Random.Range(4f,7f);
          float t = 0f;
          while (t <= 1)
          {
              t += Time.deltaTime/glitchDuration;
              float eased = Easing.Bounce.InOut(t);
              // float disturbance = 0.01f * Mathf.Cos(40f * t) + 0.01f;
              // mat.SetFloat("_FadeOutVal", (1f-t) * disturbance + t * Mathf.Max(Mathf.Sin(glitchSpeed * 2f * Mathf.PI * t), 0f));
              mat.SetFloat("_FadeOutVal", Mathf.PingPong(glitchSpeed * eased, 1.0f));
              yield return 0f;
          }
          float final = mat.GetFloat("_FadeOutVal");
          while(final > 0.001f){
            final = Mathf.Lerp(final, 0, 9f * Time.deltaTime);
            mat.SetFloat("_FadeOutVal", final );
            yield return 0;
          }
          mat.SetFloat("_FadeOutVal", 0 );

          yield return new WaitForSeconds(Random.Range(3,7));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
