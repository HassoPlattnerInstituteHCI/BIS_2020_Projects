using System.Threading.Tasks;
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
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await speechOut.Speak("This level is more complicated than the first three, but there is nothing you haven't encountered yet. You can do it.");
            UnfreezeGameObjects(); 
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 5");
        }
    }
}
