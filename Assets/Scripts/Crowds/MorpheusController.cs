using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorpheusController : MonoBehaviour
{
    private Material mat;
    [SerializeField] Renderer groundMat;
    [SerializeField] Renderer portalOfAnswerMat;
    [SerializeField] Transform envSounds;
    AudioSource[] envAudios;
    [SerializeField] AudioSource iceCrack;

    // Start is called before the first frame update
    void Start()
    {
      mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
      StartCoroutine(Glitch());

      envAudios = envSounds.GetComponents<AudioSource>();
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
      if(Vector3.Distance(CCPlayer.localPlayer.transform.position, transform.position) < 2f && !transitionComplete) { 
        //start fade out coroutine
        StartCoroutine(MorpheusTransition());
        transitionComplete = true;
      }
    }
  IEnumerator MorpheusTransition() {
      float stretch = Shader.GetGlobalFloat("_GlobalStretch");
      yield return this.xuTween((float t) => {
        Shader.SetGlobalFloat("_LevelAmt", t);
      }, 7f);

      portalOfAnswerMat.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 400f);
      this.xuTween((float t) => {
        portalOfAnswerMat.material.SetFloat("_growth", t);
        foreach(var audio in envAudios){
          audio.pitch = 1f - 0.75f * t;
        }
        Time.timeScale = 1f - 0.5f * t;
      }, 15f);
      iceCrack.PlayDelayed(2f);
      yield return new WaitForSeconds(8f);
      yield return this.xuTween((float t) => {
        float eT = Easing.Cubic.In(t);
        portalOfAnswerMat.material.SetFloat("_holeDiameter", 0.314f * eT);
        Shader.SetGlobalFloat("_GlobalStretch", stretch + 1f * t);
      }, 10f);

      bool setFog = false;
      float holeDiam = 0.314f;
      
      yield return this.xuTween((float t) => {
        if(t > 0.04 && !setFog){
          RenderSettings.fogColor = Color.black;
          setFog = true;
        }
        Vector3 pos = CCPlayer.localPlayer.transform.position;
        pos.y -= (4f + 3f * t) * 2f * Time.deltaTime;
        CCPlayer.localPlayer.transform.position = pos;

        Shader.SetGlobalFloat("_GlobalStretch", stretch + 1f * t + 1f);
        holeDiam += 2f * Time.deltaTime/(10f + 1000 * t);
        portalOfAnswerMat.material.SetFloat("_holeDiameter", holeDiam);

      }, 30f);
      portalOfAnswerMat.gameObject.SetActive(false);
      CCSceneUtils.instance.StartCoroutine(CCSceneUtils.DoFadeSceneLoadCoroutine("UnderWorldScene","CrowdScene"));

      yield return 0;
    }
    void OnDestroy(){
      Shader.SetGlobalFloat("_LevelAmt", 0f);
      Shader.SetGlobalFloat("_GlobalStretch", 0f);
    }
}
