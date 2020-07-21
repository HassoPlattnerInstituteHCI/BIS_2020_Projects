using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager3 : LevelManager
    {
        /// <summary>
        /// Starts a new round.
        /// </summary>
        /// <returns></returns>
        override public async Task ResetGame()
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await speechOut.Speak("Now there are two enemies. Say Switch to switch between enemies.");
            UnfreezeGameObjects(); 
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 4");
        }
    }
}