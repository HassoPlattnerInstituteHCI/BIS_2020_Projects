using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;
using DualPantoFramework;

namespace MarioKart
{
    public class MultiLevel : PantoBehaviour
    {
        AudioSource audioSource;
        SpeechOut speechOut = new SpeechOut();
        public ObjectOfInterest[] objectsOfInterest;

        [Tooltip("Description will be read aloud before introduction")]
        [TextArea(3, 10)]
        public String description;
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Introduce all objects of interest in order of their priority. Free both handles afterwards.
        /// </summary>
        async public Task PlayIntroduction()
        {
            Array.Sort(objectsOfInterest, ((go1, go2) => go2.priority.CompareTo(go1.priority)));

            for (int index = 0; index < objectsOfInterest.Length; index++)
            {
                await IntroduceObject(objectsOfInterest[index]);
            }
            GetPantoGameObject().GetComponent<LowerHandle>().Free();
            GetPantoGameObject().GetComponent<UpperHandle>().Free();
            IntroductionFinished?.Invoke(this);
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
                List<GameObject> children = new List<GameObject>();
                foreach (Transform child in objectOfInterest.transform)
                {
                    children.Add(child.gameObject);
                }
                children.Sort((GameObject g1, GameObject g2) => g1.name.CompareTo(g2.name));
                tasks[1] = pantoHandle.TraceObjectByPoints(children, 0.2f);
            }
            else
            {
                tasks[1] = pantoHandle.SwitchTo(objectOfInterest.gameObject, 0.2f);
            }
            await Task.WhenAll(tasks);
            await Task.Delay(500);
        }

        public delegate void OnIntroductionFinished(object sender);
        public event OnIntroductionFinished IntroductionFinished;
    }
}
