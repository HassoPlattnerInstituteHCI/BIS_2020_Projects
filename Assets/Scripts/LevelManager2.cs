using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager2 : LevelManager
    {
        override public async Task StartLevel()
        {
            await NewRoundWithDialoge(
                "An enemy protects the treasure. You can feel him using the it handle. Don't get to close to him.");
        }

        override public async Task ResetLevel()
        {
            await NewRoundWithDialoge(
                "Watch the enemy this time. He is here.");
        }

        private async Task NewRoundWithDialoge(string s)
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await speechOut.Speak(s);
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