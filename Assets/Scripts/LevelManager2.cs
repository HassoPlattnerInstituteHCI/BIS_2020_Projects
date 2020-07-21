using System.Threading.Tasks;
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
            FreezeGameObjects();
            await SpawnPlayer();
            await speechOut.Speak(
                           "An enemy protects the treasure. You can feel him using the it handle. Don't get to close to him."); 
            await SpawnEnemies();
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 3");
        }
    }
}