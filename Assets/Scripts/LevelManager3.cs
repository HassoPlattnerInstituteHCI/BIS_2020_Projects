using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager3 : LevelManager
    {
        override public async Task StartLevel()
        {
            await NewRoundWithDialoge("LM3-1");
        }

        override public async Task ResetLevel()
        {
            await NewRoundWithDialoge("LM3-2");
        }

        private async Task NewRoundWithDialoge(string s)
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await PlayTextAudio(s);
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 4");
        }
    }
}