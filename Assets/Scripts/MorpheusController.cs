using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorpheusController : MonoBehaviour
{
    private Material mat;
    [SerializeField] GameObject ground;

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
    bool transitionComplete = false;
    void Update()
    {
      if(Vector3.Distance(CCPlayer.localPlayer.transform.position, transform.position) < 5f && !transitionComplete) { 
        //start fade out coroutine
        StartCoroutine(MorpheusTransition());
        transitionComplete = true;
      }
    }

    IEnumerator MorpheusTransition() {
      float stretch = Shader.GetGlobalFloat("_GlobalStretch");
      yield return this.xuTween((float t) => {
        Shader.SetGlobalFloat("_GlobalStretch", stretch + 3f * t);
      }, 5f);
      ground.SetActive(false); //fall
      yield return this.xuTween((float t) => {
        Vector3 pos = CCPlayer.localPlayer.transform.position;
        pos.y -= 10f * t * Time.deltaTime;
        CCPlayer.localPlayer.transform.position = pos;
      }, 10f);
      
      yield return 0;
    }
}
