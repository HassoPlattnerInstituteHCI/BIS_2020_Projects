using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SurgeonSim
{
    public class PlayerLogic : MonoBehaviour
    {
        private PantoHandle upperHandle;
        private PantoHandle lowerHandle;

        AudioSource audioSource;
        public AudioClip heartbeatClip;

        public int startBPM = 60;
        public int endBPM = 220;
        float bpmCoefficient;
        public float bps = 1;
        float nextHeartbeat;
        Health health;

        SpeechOut speechOut;
        SpeechIn speechIn;

        private GameObject scalpel;
        private GameObject kidney;
        private GameObject player;
        private int lives = 2;
        bool strike = false;
        private bool attached = false;

        Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };

        private void Awake()
        {
            speechOut = new SpeechOut();
            speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        }

        async void onRecognized(string message)
        {
            Debug.Log("SpeechIn recognized: " + message);
        }

        void Start()
        {
            upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();

            scalpel = GameObject.Find("Scalpel");
            scalpel.SetActive(false);
            Debug.Log(scalpel);
            kidney = GameObject.Find("KidneyLeft");
            Debug.Log(kidney);
            player = GameObject.Find("Player");
            /* health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();

            bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2); */
        }

        void Update()
        {
            transform.position = upperHandle.HandlePosition(transform.position);

            if (attached)
            {
                kidney.transform.position = upperHandle.HandlePosition(kidney.transform.position);
                Debug.Log(kidney.transform.position);
                Debug.Log(player.transform.position);
            }

            if (kidney.transform.position.x < 7.23f && kidney.transform.position.z < -16.06f)
            {
                speechOut.Speak("Super.");
                AsyncOperation async = SceneManager.LoadSceneAsync(1);
                async.allowSceneActivation = true;
            }

            /* if (health.healthPoints > 0 && health.healthPoints <= 2 * health.maxHealth / 3)
            {
                if (nextHeartbeat > bps)
                {
                    float bpm = bpmCoefficient * Mathf.Pow(health.healthPoints - health.maxHealth, 2) + startBPM;
                    bps = 60f / bpm;
                    audioSource.PlayOneShot(heartbeatClip);
                    nextHeartbeat = 0;
                }
                else
                {
                    nextHeartbeat += Time.deltaTime;
                }
            } */
            /*if (rightKidneySeparated)
            {
                GameObject kidney = GameObject.Find("KidneyRight");
                kidney.transform.position = GameObject.Find("MeHandlePrefab(Clone)").transform.position;
            }
            if (leftKidneySeparated)
            {
                GameObject kidney = GameObject.Find("KidneyLeft");
                kidney.transform.position = GameObject.Find("MeHandlePrefab(Clone)").transform.position;
            }*/
        }

        private async void OnCollisionEnter(Collision collision)
        {
            string name = collision.gameObject.name;

            if (name.Contains("Fill") || name.Contains("group"))
            {

            }
            else
            {
                if (name == "KidneyLeft")
                {

                }
                else
                {
                    await speechOut.Speak(collision.gameObject.name);
                }
            }

            if ("KidneyLeft" == collision.gameObject.name && !attached)
            {
                if (scalpel.activeSelf)
                {
                    attached = true;
                    // await speechOut.Speak("Say hand.");
                    // await speechIn.Listen(new Dictionary<string, KeyCode>() { { "hand", KeyCode.H } });
                    scalpel.SetActive(false);
                    await speechOut.Speak("Move the kidney to the bottom right corner.");
                    await lowerHandle.MoveToPosition(new Vector3(7.23f, 0f, -16.06f), 0.3f, false);
                }
                else
                {
                    await speechOut.Speak("Now say scalpel.");
                    await speechIn.Listen(new Dictionary<string, KeyCode>() { { "scalpel", KeyCode.S } });
                    scalpel.SetActive(true);
                    await speechOut.Speak("Make a cut to the kidney.");
                    /*UnityEditor.EditorApplication.isPlaying = false;
                    Application.Quit();*/
                }
            }



            if (!collision.gameObject.name.Contains("group") && scalpel.activeSelf)
            {
                Debug.Log(name);
                if (strike)
                {
                    await speechOut.Speak("Oh no, you messed up! Game over!");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                else
                {
                    await speechOut.Speak("Be careful! You got one more chance.");
                    strike = true;
                }
            }
        }
    }
}
