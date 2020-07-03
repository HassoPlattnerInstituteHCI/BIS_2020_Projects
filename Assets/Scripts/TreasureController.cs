using SpeechIO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Stealth
{


    public class TreasureController : MonoBehaviour
    {
        private AudioSource audioS;
        public GameObject player;
        private bool found;
        SpeechOut speechOut;
        private LevelManager script;

        Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };
        // Start is called before the first frame update
        void Start()
        {
            audioS = gameObject.GetComponent<AudioSource>();
            //player = GameObject.Find("Player");
            speechOut = new SpeechOut();
            script = GameObject.Find("Panto").GetComponent<LevelManager>();

        }


        // Update is called once per frame
        void Update()
        {
            if (!found && player.activeSelf)
            {
                if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= 1)
                {
                    found = true;
                    GameOver();
                }
                audioS.pitch = 6 / Vector3.Distance(gameObject.transform.position, player.transform.position);

            }

        }

        async Task GameOver()
        {
            await speechOut.Speak("Congratulations.");



            await speechOut.Speak("Thanks for playing DuelPanto.");


            Application.Quit();
        }
        public void OnApplicationQuit()
        {
            speechOut.Stop(); //Windows: do not remove this line.

        }
    }
}