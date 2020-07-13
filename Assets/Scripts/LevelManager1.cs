using System.Threading.Tasks;
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
            DeactivateGameObjects();
            await speechOut.Speak("Your are here.");
            await SpawnPlayer();
            await speechOut.Speak("You can move using the me handle. Follow the ticking sound and find treasure. Find a way around the obstacles.");
            ActivateGameObjects();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene (sceneName:"Level 2");
        }
    }
}