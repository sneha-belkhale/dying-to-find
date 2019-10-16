/*
An assortment of random useful functions/classes/stuff

Also includes some Debug developer settings

TODO convert some of these to extension methods!

*/
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

//TODO try and put camera stuff in here

public static class XUExtensions
{


    public delegate void TFunction(float t);

    public static Coroutine xuTween(this MonoBehaviour thiss, TFunction tfunc, float dur)
    {
        return thiss.StartCoroutine(genericT(tfunc, dur));
    }

    public static Quaternion AsQuaternion(this Vector4 thiss)
    {
        return new Quaternion(thiss.x, thiss.y, thiss.z, thiss.w);
    }


    public static IEnumerator genericT(TFunction tfunc, float dur)
    {
        float startTime = Time.time;

        while (Time.time < startTime + dur)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / dur);
            tfunc(t);
            yield return new WaitForEndOfFrame();
        }
        //force call with 1
        tfunc(1);
    }

    public delegate void NoArgNoRetFunction();

    public static void CopyTo<T>(this List<T> thiss, List<T> dest)
    {
        dest.Clear();
        for (int i = 0; i < thiss.Count; i++)
        {
            dest.Add(thiss[i]);
        }
    }

    public static Color withAlpha(this Color thiss, float a)
    {
        Color ret = thiss;
        ret.a = a;
        return ret;
    }

    public static Vector3 withX(this Vector3 thiss, float x)
    {
        Vector3 ret = thiss;
        ret.x = x;
        return ret;
    }
    public static Vector3 withY(this Vector3 thiss, float y)
    {
        Vector3 ret = thiss;
        ret.y = y;
        return ret;
    }

    public static Vector3 withZ(this Vector3 thiss, float z)
    {
        Vector3 ret = thiss;
        ret.z = z;
        return ret;
    }

    public static Vector3 asVector3(this Vector4 thiss)
    {
        return new Vector4(thiss.x, thiss.y, thiss.z);
    }

    public static Vector4 asVector4(this Vector3 thiss, float w = 1)
    {
        return new Vector4(thiss.x, thiss.y, thiss.y, w);
    }

    public static void delayedFunction(this MonoBehaviour thiss, NoArgNoRetFunction func, float delay)
    {
        thiss.StartCoroutine(delayedFunctionRoutine(func, delay));
    }

    static IEnumerator delayedFunctionRoutine(NoArgNoRetFunction func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func();
    }

    //public static string toSti(this ResourceType grade)
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public static T GetComponentInParentOrSelf<T>(this MonoBehaviour thiss)
    {
        T ret = thiss.GetComponent<T>();
        ret = ret == null ? thiss.GetComponentInParent<T>() : ret;
        return ret;
    }

    public static T GetComponentInParentOrSelf<T>(this Transform thiss)
    {
        T ret = thiss.GetComponent<T>();
        ret = ret == null ? thiss.GetComponentInParent<T>() : ret;
        return ret;
    }



}

public static class XUGizmos
{
    public static void DrawSphereCast(Vector3 start, float radius, Vector3 direction, float length)
    {
        //Gizmos.color = color;

        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(start + (direction * length), radius);

        Quaternion[] rots = { Quaternion.Euler(-90, 0, 0), Quaternion.Euler(90, 0, 0), Quaternion.Euler(0, -90, 0), Quaternion.Euler(0, 90, 0) };
        foreach (Quaternion rot in rots)
        {
            Vector3 edgeLineOffset = rot * direction;
            edgeLineOffset *= radius;

            Gizmos.DrawLine(start + edgeLineOffset, start + direction * length + edgeLineOffset);

        }
    }
}

public class XUUtil
{
    public static float ConvertRange(float originalMin, float originalMax, float targetMin, float targetMax, float number)
    {
        float ret = (number - originalMin) / (originalMax - originalMin) * (targetMax - targetMin) + targetMin;

        ret = Mathf.Clamp(ret, targetMin, targetMax);
        return ret;
    }

    public static void DrawCenteredText(Vector2 offset, string text, int fontSize, Color fontColor, string font)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = fontColor;
        style.fontSize = fontSize;
        style.font = (Font)Resources.Load("Fonts/" + font);
        Vector2 size = style.CalcSize(new GUIContent(text));
        Vector2 position = new Vector2(Screen.width, Screen.height) / 2 - size / 2;
        position += offset;
        GUI.Label(new Rect(position.x, position.y, size.x, size.y), text, style);
    }

    public static void DrawText(Vector2 position, string text, int fontSize, Color fontColor, string font = null)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = fontColor;
        style.fontSize = fontSize;

        if (font != null)
        {
            style.font = (Font)Resources.Load("Fonts/" + font);
        }

        Vector2 size = style.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(position.x, position.y, size.x, size.y), text, style);
    }

    public static void DrawTextAtWorldPosition(Vector3 worldPt, string text, int fontSize, Color fontColor, Camera cam, Vector2 offset = new Vector2())
    {

        Vector3 position = cam.WorldToScreenPoint(worldPt);//Camera.main.WorldToScreenPoint(worldPt);
        position.y = Screen.height - position.y;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = fontColor;
        style.fontSize = fontSize;
        //style.font = (Font)Resources.Load("Fonts/" + font);
        Vector2 size = style.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(position.x + offset.x - size.x / 2, position.y - offset.y - size.y / 2, size.x, size.y), text, style);
        //GUI.Label(new Rect(position.x, position.y, size.x, size.y), text, style);
    }




    /*public class StoredTransform
    {
        public Vector3 position;
        public Quaternion rotation;


        public Vector3 forward
        {
            get { return rotation * Vector3.forward; }
            set { rotation = Quaternion.LookRotation(value); }
        }

   
        
        public StoredTransform (Transform t)
        {
            this.position = t.position;
            this.rotation = t.rotation;
        }

        public static implicit operator UnityEngine.Transform(StoredTransform t)  // implicit digit to byte conversion operator
        {
            System.Console.WriteLine("conversion occurred");
            return new StoredTransform(t);
        }
    }*/
}