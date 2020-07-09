using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using System;
using DualPantoFramework;

namespace dualLayouting {
    public class AudioManager : MonoBehaviour
    {

        private SpeechIn speechIn;
        private SpeechOut speechOut; 
        private Action<string> onSelect;
        private Action<string> onCreate;
        Action<string> onDelete;
        Action<string> onShow;
        private Action onList;
        private Action onDone;
        private GameObject[] elements;
        private static List<string> supportedElements = new List<string> {
            "Tree",
            "Balloon",
            "Congratulations",
            "Fireworks",
            "Champagne",
            "Cake"
        };

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Audiomanager initialized");
            speechOut = new SpeechOut();
            speechIn = new SpeechIn(OnRecognize);
        }

        async public Task Say(string text)
        {
            await speechOut.Speak(text);
        }
        public void SetCallbacks(Action<string> selectCallback,
                                 Action<string> createCallback,
                                 Action<string> deleteCallback,
                                 Action listCallback,
                                 Action<string> showCallback,
                                 Action doneCallback)
        {
            onSelect = selectCallback;
            onCreate = createCallback;
            onDelete = deleteCallback;
            onList = listCallback;
            onShow = showCallback;
            onDone = doneCallback;
        }

        async public void UpdateCommands(GameObject[] elements)
        {
            this.elements = elements;

            List<string> newCommands = new List<string> {};

            foreach (GameObject element in elements)
            {
                newCommands.Add($"Select {element.name}");
                newCommands.Add($"Delete {element.name}");
                newCommands.Add($"Show {element.name}");
            }

            // permanent commands
            newCommands.Add("List Elements");
            newCommands.Add("Done");

            foreach (string element in supportedElements)
            {
                newCommands.Add($"Create {element}");
            }

            speechIn.StartListening(newCommands.ToArray());
        }

        private void OnRecognize(string command){
            Debug.Log(command);
            if (command == "Done")
            {
                onDone();
                return;
            }
            if (command == "List Elements")
            {
                onList();
                return;
            }

            var commandHandlers = new Dictionary<string, Action<string>> {
                {"Create", onCreate},
                {"Select", onSelect},
                {"Show", onShow},
                {"Delete", onDelete}
            };

            foreach (var handler in commandHandlers)
            {
                if (command.StartsWith(handler.Key))
                {
                    string parameter = command.Substring(handler.Key.Length + 1);
                    handler.Value(parameter);
                    return;
                }
            }
        } 

        public void OnApplicationQuit()
        {
            if (speechIn != null)
            {
                speechIn.StopListening();
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}