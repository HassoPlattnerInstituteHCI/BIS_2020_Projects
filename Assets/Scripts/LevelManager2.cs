using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager2 : LevelManager
    {
        /// <summary>
        /// Starts a new round.
        /// </summary>
        /// <returns></returns>
        override public async Task ResetGame()
        {
            DeactivateGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await speechOut.Speak(
                "An enemy protects the treasure. You can feel him using the it handle. Don't get to close to him.");
            upperHandle.Free();
            ActivateGameObjects();
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 3");
        }
    }
}