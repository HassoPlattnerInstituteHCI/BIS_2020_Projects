using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager1 : LevelManager
    {
        /// <summary>
        /// Starts a new round.
        /// </summary>
        /// <returns></returns>
        override public async Task ResetGame()
        {
            await speechOut.Speak("Spawning player");
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);

            await speechOut.Speak("You can move using the me handle. Follow the ticking sound and find treasure while avoiding the obstacles.");
            
            upperHandle.Free();
            player.SetActive(true);
        }

        async public Task Success()
        {
            SceneManager.LoadScene (sceneName:"Level 2");
        }
    }
}