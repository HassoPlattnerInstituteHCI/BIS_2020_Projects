﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;
using UnityEditor;
using DualPantoFramework;

/// <summary>
/// A level that can be introduced to the player. You could use one of these for each scene, or for each room in a scene.
/// </summary>
public class Level : PantoBehaviour
{
    AudioSource audioSource;
    SpeechOut speechOut = new SpeechOut();
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Introduce all objects of interest in order of their priority. Free both handles afterwards.
    /// </summary>
    async public Task PlayIntroduction()
    {
        ObjectOfInterest[] gos = UnityEngine.Object.FindObjectsOfType<ObjectOfInterest>();
        Array.Sort(gos, ((go1, go2) => go2.priority.CompareTo(go1.priority)));

        for (int index = 0; index < gos.Length; index++)
        {
            await IntroduceObject(gos[index]);
        }
        GetPantoGameObject().GetComponent<LowerHandle>().Free();
        GetPantoGameObject().GetComponent<UpperHandle>().Free();
    }

    async private Task IntroduceObject(ObjectOfInterest objectOfInterest)
    {
        Task[] tasks = new Task[2];
        tasks[0] = speechOut.Speak(objectOfInterest.description);

        PantoHandle pantoHandle = objectOfInterest.isOnUpper
            ? (PantoHandle)GetPantoGameObject().GetComponent<UpperHandle>()
            : (PantoHandle)GetPantoGameObject().GetComponent<LowerHandle>();

        if (objectOfInterest.traceShape)
        {
            Debug.Log("tracing shape");
            tasks[1] = TraceObject(objectOfInterest.gameObject, pantoHandle);
        }
        else
        {
            tasks[1] = pantoHandle.SwitchTo(objectOfInterest.gameObject, 0.2f);
        }
        await Task.WhenAll(tasks);
        await Task.Delay(500);
    }

    public Task TraceObject(GameObject obj, PantoHandle handle = null)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in obj.transform)
        {
            if (child.gameObject.tag == "Corner")
            {
                children.Add(child.gameObject);
            }
        }
        children.Sort((GameObject g1, GameObject g2) => g1.name.CompareTo(g2.name));
        if (children.Count > 0)
        {
            return (handle ?? (PantoHandle)GetPantoGameObject().GetComponent<LowerHandle>()).TraceObjectByPoints(children, 0.2f);
        }
        else
        {
            return (handle ?? (PantoHandle)GetPantoGameObject().GetComponent<LowerHandle>()).SwitchTo(obj.gameObject, 0.2f);
        }
    }
}