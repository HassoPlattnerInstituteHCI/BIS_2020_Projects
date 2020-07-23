using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager2 : LevelManager
    {
        override public async Task StartLevel()
        {
            await NewRoundWithDialoge("LM2-1");
        }

        override public async Task ResetLevel()
        {
            await NewRoundWithDialoge("LM2-2");
        }

        private async Task NewRoundWithDialoge(string s)
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await PlayTextAudio(s);
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