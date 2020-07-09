using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager4 : LevelManager
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
            await speechOut.Speak("This level is more complicated than the first three, but there is nothing you haven't encountered yet. You can do it.");
            upperHandle.Free();
            ActivateGameObjects(); 
            ListenToSwitch();
        }

        override async public Task Success()
        {
            await speechOut.Speak("Congratutations. You finished level 4! That's the end of the game for now. Thanks for playing Stealth Panto.");
        }
    }
}
