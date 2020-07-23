using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using System.Linq;
using UnityEngine.SceneManagement;
using DualPantoFramework;

namespace PantoDrawing
{
    public class GameManager : MonoBehaviour
    {
        static GameManager instance;
        UpperHandle upperHandle;
        LowerHandle lowerHandle;
        private SpeechIn speechIn;
        private SpeechOut speechOut;
        public int level;
        public static LevelMaster levelMaster;
        private static LineDraw lineDraw;

        GameObject level1;
        GameObject level2;
        GameObject level4;
     
        public Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>() {
            { "circle", () => {
                    lineDraw.CreateCircle();
                }},
            { "rectangle", () => {
                    lineDraw.CreateRectangle();
                }},
            { "triangle", () => {
                    lineDraw.CreateTriangle();
                }},
            { "yes", () => {
                    levelMaster.ready = true;
                }},
            { "show all", () => {
                    lineDraw.ShowLines();
                }},
            { "repeat" , () => {}},
            { "options" , () => {}},
            { "quit" , () => {}}
        };

        public bool levelMode;

        //public FirstLevel firstLevel; um die level ggf auszulagern in ein eigenes Skript aber das mag grad nciht

        void Awake()
        {
            speechIn = new SpeechIn(onRecognized, keywords.Keys.ToArray());
            speechOut = new SpeechOut();
        }
        

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();
            lineDraw = GameObject.Find("Panto").GetComponent<LineDraw>();
            Debug.Log("Before Introduction");
            speechIn.StartListening(keywords.Keys.ToArray());


            level1 = GameObject.Find("Level1");
            level1.SetActive(false);

            level2 = GameObject.Find("Level2");
            level2.SetActive(false);

            level4 = GameObject.Find("Level4");
            level4.SetActive(false);
            
            RegisterColliders();
            if(levelMode)
            {
                Debug.Log(levelMode);
                Levels();
            } else
            {
                lineDraw.canDraw = true;
            }
        }

        async void onRecognized(string message)
        {
            Debug.Log(message);
            switch (message)
            {
                case "repeat":
                    await speechOut.Repeat();
                    break;
                case "quit":
                    await speechOut.Speak("Thanks for using our application. Closing down now...");
                    OnApplicationQuit();
                    Debug.Log("test");
                    Application.Quit();
                    break;
                case "options":
                    string commandlist = "";
                    foreach (KeyValuePair<string, System.Action> command in keywords)
                    {
                        commandlist += command.Key + ", ";
                    }
                    await speechOut.Speak("currently available commands: " + commandlist);
                    break;
                default:
                    defaultSpeech(message);
                    break;
            }
        }

        private async void defaultSpeech(string text)
        {
            System.Action keywordAction;
            // if the keyword recognized is in our dictionary, call that Action.
            if (keywords.TryGetValue(text, out keywordAction))
            {
                keywordAction.Invoke();
                await speechOut.Speak(text);
            }
        }
        
        private void OnApplicationQuit()
        {
            speechOut.Stop();
            speechIn.StopListening();
        }

        
        async void RegisterColliders()
        {
            await Task.Delay(1000);
            PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in colliders)
            {
                collider.CreateObstacle();
                collider.Enable();
            }
        }

        async void Levels()
        {
            switch(level)
            {
                case 1:
                    Debug.Log("Starting Level1");
                    level1.SetActive(true);
                    levelMaster = (new GameObject("Level1")).AddComponent<Level1>();
                    await levelMaster.StartLevel(lineDraw, speechIn, speechOut);
                    level++;
                    Levels();
                    break;
                case 2:
                    level2.SetActive(true);
                    levelMaster = (new GameObject("Level2")).AddComponent<Level2>();
                    await levelMaster.StartLevel(lineDraw, speechIn, speechOut);
                    level++;
                    Levels();
                    break;
                case 3:
                    levelMaster = (new GameObject("Level3")).AddComponent<Level3>();
                    await levelMaster.StartLevel(lineDraw, speechIn, speechOut);
                    level++;
                    Levels();
                    break;
                case 4:
                    level4.SetActive(true);
                    levelMaster = (new GameObject("Level4")).AddComponent<Level4>();
                    await levelMaster.StartLevel(lineDraw, speechIn, speechOut);
                    level++;
                    Levels();
                    break;
                case 5:
                    ResetSpeech();
                    level1.SetActive(false);
                    level2.SetActive(false);
                    level4.SetActive(false);
                    lineDraw.ResetDrawingArea();
                    levelMaster = (new GameObject("Level5")).AddComponent<Level5>();
                    await levelMaster.StartLevel(lineDraw, speechIn, speechOut);
                    level++;
                    Levels();
                    break;
                case 6:
                    ResetSpeech();
                    lineDraw.ResetDrawingArea();
                    levelMaster = (new GameObject("Level6")).AddComponent<Level6>();
                    await levelMaster.StartLevel(lineDraw, speechIn, speechOut);
                    level++;
                    Levels();
                    break;
                default:
                    //TODO
                    Debug.Log("Default level case");
                    lineDraw.canDraw = true;
                    break;
            }
        }
        void ResetSpeech()
        {
            keywords = new Dictionary<string, System.Action>() {
            { "circle", () => {
                    lineDraw.CreateCircle();
                }},
            { "yes", () => {
                    levelMaster.ready = true;
                }},
            { "repeat" , () => {}},
            { "options" , () => {}},
            { "quit" , () => {}}
            };
            speechIn = new SpeechIn(onRecognized, keywords.Keys.ToArray());
        }

        public void AddVoiceCommand(string commandKey, System.Action command){
            keywords.Add(commandKey, command);
            speechIn.StartListening(keywords.Keys.ToArray());
        }
}
}
