using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager4 : LevelManager
    {
        override public async Task StartLevel()
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await PlayTextAudio("LM4-1");
            UnfreezeGameObjects(); 
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 5");
        }
    }
}
