using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stealth
{
    public abstract class LevelManager : MonoBehaviour
    {
        public GameObject player;
        public GameObject[] enemies;
        public GameObject currentEnemy;
        public int EnemyIndex = 0;
        public Transform playerSpawn;
        public UpperHandle upperHandle;
        public LowerHandle lowerHandle;
        public SpeechIn speechIn;
        public SpeechOut speechOut;
        private GameObject _treasure;
        private TreasureController _treasureController;
        private bool _hasHitObstacle;

        public Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>()
        {
            {"yes", KeyCode.Y},
            {"no", KeyCode.N},
            {"done", KeyCode.D},
            {"switch", KeyCode.E}
        };

        void Awake()
        {
            speechIn = new SpeechIn(OnRecognized, commands.Keys.ToArray());
            speechOut = new SpeechOut();
        }

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();

            Introduction();
        }

        async void Introduction()
        {
            await Task.Delay(1000);
            RegisterColliders();

            ActivateGameObjects();
            await StartLevel();
        }

        void RegisterColliders()
        {
            PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in colliders)
            {
                Debug.Log(collider);
                collider.CreateObstacle();
                collider.Enable();
            }
        }

        abstract public Task StartLevel();

        /// <summary>
        /// Starts a new round.
        /// </summary>
        /// <returns></returns>
        virtual public async Task ResetLevel()
        {
            FreezeGameObjects();
            await SpawnPlayer();
            await SpawnEnemies();
            UnfreezeGameObjects();
            ListenToSwitch();
        }

        /// <summary>
        /// Complete the level.
        /// Supposed for cleanup and redirecting to the next level.
        /// </summary>
        /// <returns></returns>
        abstract public Task Success();


        async void OnRecognized(string message)
        {
            Debug.Log("SpeechIn recognized: " + message);
        }

        public void OnApplicationQuit()
        {
            speechOut.Stop(); //Windows: do not remove this line.
            speechIn.StopListening(); // [macOS] do not delete this line!
        }

        public async Task ListenToSwitch()
        {
            string response = await speechIn.Listen(commands);
            if (response == "switch")
            {
                await ToggleEnemies();
            }

            ListenToSwitch();
        }

        async Task ToggleEnemies()
        {
            if (EnemyIndex < enemies.Length - 1)
            {
                EnemyIndex++;
            }
            else EnemyIndex = 0;

            currentEnemy = enemies[EnemyIndex];
            await lowerHandle.SwitchTo(currentEnemy, 0.3f);
        }

        public void FreezeGameObjects()
        {
            player.GetComponent<PlayerController>().Freeze();
            foreach (GameObject en in enemies)
            {
                en.GetComponent<EnemyController>().Freeze();
            }

            GetTreasureController().tickingAudioSource.Pause();
        }

        public void ActivateGameObjects()
        {
            player.SetActive(true);
            foreach (GameObject en in enemies)
            {
                en.SetActive(true);
            }
        }

        public void UnfreezeGameObjects()
        {
            player.SetActive(true);
            player.GetComponent<PlayerController>().Unfreeze();
            foreach (GameObject en in enemies)
            {
                en.GetComponent<EnemyController>().Unfreeze();
            }

            GetTreasureController().tickingAudioSource.Play();
        }

        public async Task SpawnPlayer()
        {
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);
        }

        public async Task SpawnEnemies()
        {

            EnemyIndex = 0;
            if (enemies.Length > 0)
            {
                currentEnemy = enemies[EnemyIndex];
                await lowerHandle.SwitchTo(currentEnemy, 0.3f);
            }
        }

        public GameObject GetTreasure()
        {
            if (_treasure == null)
            {
                _treasure = GameObject.FindWithTag("Target");
            }

            return _treasure;
        }

        public TreasureController GetTreasureController()
        {
            if (_treasureController == null)
            {
                _treasureController = GetTreasure().GetComponent<TreasureController>();
            }

            return _treasureController;
        }

        public async Task OnObstacleHit()
        {
            if (!_hasHitObstacle)
            {
                _hasHitObstacle = true;
                await OnFirstObstacleHit();
            }
        }

        virtual public async Task OnFirstObstacleHit()
        {
            return;
        }

        public async Task MoveItHandleToTreasure()
        {
            await lowerHandle.SwitchTo(GetTreasure(), 0.3f);
        }
    }
}