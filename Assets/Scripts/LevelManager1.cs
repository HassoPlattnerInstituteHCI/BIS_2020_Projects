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
            FreezeGameObjects();
            await speechOut.Speak("Welcome to Stealth Panto. You are a ninja searching for treasure in a dangerous medieval city.");
            await speechOut.Speak("Your are here.");
            await SpawnPlayer();
            await speechOut.Speak("The treasure is here.");
            await MoveItHandleToTreasure();
            await speechOut.Speak("You can move using the me handle. Follow the ticking sound and find treasure.");
            UnfreezeGameObjects();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene (sceneName:"Level 2");
        }

        override public async Task OnFirstObstacleHit()
        {
            await speechOut.Speak("This is an obstacle. Find a way around it.");
        }
    }
}