using SpeechIO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class TreasureController : MonoBehaviour
    {
        public AudioSource tickingAudioSource;
        public AudioSource successAudioSource;

        public GameObject player;
        private bool found;
        SpeechOut speechOut;
        private LevelManager script;

        // Start is called before the first frame update
        void Start()
        {
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

                tickingAudioSource.pitch =
                    6 / Vector3.Distance(gameObject.transform.position, player.transform.position);
            }
        }

        async Task GameOver()
        {
            successAudioSource.Play();
            await speechOut.Speak("Congratulations. You finished the level.");

            // TODO: Improve using inheritance
            if (SceneManager.GetActiveScene().name == "Level 1")
            {
                LevelManager1 script = GameObject.Find("Panto").GetComponent<LevelManager1>();
                await script.Success();
            }
            else if (SceneManager.GetActiveScene().name == "Level 2")
            {
                LevelManager2 script = GameObject.Find("Panto").GetComponent<LevelManager2>();
                await script.Success();
            }
            else if (SceneManager.GetActiveScene().name == "Level 1")
            {
                await speechOut.Speak("congratulations. You completed the game.");
                await speechOut.Speak("Thanks for playing DuelPanto.");
                Application.Quit();
            }
        }

        public void OnApplicationQuit()
        {
            speechOut.Stop(); //Windows: do not remove this line.
        }
    }
}