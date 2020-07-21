using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager3 : LevelManager
    {
        override public async Task StartLevel()
        {
            await NewRoundWithDialoge("Now there are two enemies. Say Switch to switch between enemies.");
        }

        override public async Task ResetLevel()
        {
            await NewRoundWithDialoge("Remember: Say Switch to switch between enemies.");
        }

        private async Task NewRoundWithDialoge(string s)
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            await speechOut.Speak(s);
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        override async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 4");
        }
    }
}