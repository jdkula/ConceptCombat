using System;
using System.Collections;
using System.Collections.Generic;
using Game.UserInterface;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameObject DebugSphere;

        public GameObject FloorPrefab;
        public GameObject BlockPrefab;
        public GameObject DoorPrefab;
        public GameObject SkullPrefab;
        public GameObject PlayerPrefab;
        public CameraHelper Camera;
        public AudioSource WinSound;
        public EnemyUserInterface EUI;
        public PlayerUserInterface PUI;
        public GameObject Player;

        public int EnemiesLeft;
        public float TileDimension;
        public Vector3 TileScale;

        private CameraController _camController;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Instance = this;
        }

        void Start()
        {
            _generateLevel();
            StartCoroutine(WaitCamera(_camController));
        }

        /// <summary>
        /// Gives the user a sneak peak at the level, then begins following the player.
        /// </summary>
        private IEnumerator WaitCamera(CameraController camController)
        {
            yield return new WaitForSecondsRealtime(Constants.CameraOverviewTime);
            camController.Following = true;
        }

        /// <summary>
        /// Given an array of Vector3s representing the four corners
        /// of the screen the camera can see, turns it into a square
        /// by reducing the largest dimension, and returns it.
        /// </summary>
        public static Vector3[] Squarify(Vector3[] notSquare, CameraHelper cam)
        {
            float height = cam.GetFustrumHeight(cam.transform.position.y);
            float width = cam.GetFustrumWidth(cam.transform.position.y);

            if (Math.Abs(width - height) < 0.01)
            {
                return notSquare;
            }

            if (width > height)
            {
                float halfdiff = (width - height) / 2f;
                return new[]
                {
                    new Vector3(notSquare[0].x - halfdiff, notSquare[0].y, notSquare[0].z),
                    new Vector3(notSquare[1].x + halfdiff, notSquare[1].y, notSquare[1].z),
                    new Vector3(notSquare[2].x + halfdiff, notSquare[2].y, notSquare[2].z),
                    new Vector3(notSquare[3].x - halfdiff, notSquare[3].y, notSquare[3].z)
                };
            }
            else
            {
                float halfdiff = (height - width) / 2f;
                return new[]
                {
                    new Vector3(notSquare[0].x, notSquare[0].y, notSquare[0].z - halfdiff),
                    new Vector3(notSquare[1].x, notSquare[1].y, notSquare[1].z - halfdiff),
                    new Vector3(notSquare[2].x, notSquare[2].y, notSquare[2].z + halfdiff),
                    new Vector3(notSquare[3].x, notSquare[3].y, notSquare[3].z + halfdiff)
                };
            }
        }

        private static Vector3 GetTransform(Vector3[] workArea, float tileDimensions, int x, int z)
        {
            Vector3 topLeft = new Vector3(workArea[1].x + tileDimensions / 2, workArea[1].y,
                workArea[1].z - tileDimensions / 2);

            return new Vector3(topLeft.x + tileDimensions * x, topLeft.y, topLeft.z - tileDimensions * z);
        }

        /// <summary>
        /// Wins the game by touching the door
        /// </summary>
        public IEnumerator OnDoorTouch(GameObject door)
        {
            WinSound.Play();
            door.GetComponent<Door>().Open();
            yield return new WaitForSecondsRealtime(WinSound.clip.length);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        public void NextLevel(GameObject doorInstance)
        {
            StartCoroutine(OnDoorTouch(doorInstance));
        }

        public void EnterBattle(Entity player, Entity enemy)
        {
            player.BattlePositions();
            enemy.BattlePositions();
            player.Battle(enemy);
            enemy.Battle(player);
            EUI.Battle(enemy);
            PUI.Battle(player);

            _camController.Battle(player, enemy);

            GetComponent<AudioController>().Battle();
        }

        public void ExitBattle(Entity player, Entity enemy)
        {
            Enemy.Unfreeze();
            player.ExitBattle();
            enemy.ExitBattle();
            EUI.ExitBattle();
            PUI.ExitBattle();
            _camController.ExitBattle();
            GetComponent<AudioController>().ExitBattle();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void _generateLevel()
        {
            Random rgen = new Random();

            Vector3[] workArea = Squarify(Camera.GetFustrumBounds(Camera.transform.position.y), Camera);
            int tilesPerRow = Mathf.CeilToInt(Mathf.Sqrt(Constants.TileCountPerScreen));
            TileDimension = (workArea[0].x - workArea[1].x) / tilesPerRow;
            TileScale = new Vector3(TileDimension, TileDimension, 1);


            GameObject[,] tiles = new GameObject[tilesPerRow, tilesPerRow];

            int currentExitPossibility = 0;
            int[] xExitPossibilities = new int[tilesPerRow * 4 - 8];
            int[] yExitPossibilities = new int[tilesPerRow * 4 - 8];

            List<int[]> entityPlacementPossibilities = new List<int[]>();

            for (int x = 0; x < tilesPerRow; x++)
            {
                for (int z = 0; z < tilesPerRow; z++)
                {
                    bool isBlock = rgen.NextDouble() < Constants.BlockChance;
                    GameObject toInstantiate = isBlock ? BlockPrefab : FloorPrefab;
                    if (z == 0 || x == 0 || x == tilesPerRow - 1 || z == tilesPerRow - 1)
                    {
                        toInstantiate = BlockPrefab;
                        isBlock = true;
                        if (!(x == 0 && z == 0) && !(x == 0 && z == tilesPerRow - 1) &&
                            !(x == tilesPerRow - 1 && z == 0) &&
                            !(x == tilesPerRow - 1 && z == tilesPerRow - 1))
                        {
                            xExitPossibilities[currentExitPossibility] = x;
                            yExitPossibilities[currentExitPossibility] = z;
                            currentExitPossibility++;
                        }
                    }

                    if (!isBlock) entityPlacementPossibilities.Add(new[] {x, z});
                    tiles[x, z] = Instantiate(toInstantiate, GetTransform(workArea, TileDimension, x, z),
                        FloorPrefab.transform.rotation);
                    tiles[x, z].transform.localScale = TileScale;
                }
            }

            int exit = rgen.Next(0, xExitPossibilities.Length);
            int xExit = xExitPossibilities[exit];
            int yExit = yExitPossibilities[exit];
            GameObject inTheWay = tiles[xExit, yExit];
            Transform oldTransform = inTheWay.transform;
            Destroy(inTheWay);

            GameObject doorGo = Instantiate(DoorPrefab, oldTransform.position, oldTransform.rotation);
            doorGo.transform.localScale = TileScale;
            tiles[xExit, yExit] = doorGo;
            int xDestroy = -1;
            int yDestroy = -1;
            if (yExit == 0)
            {
                if (tiles[xExit, yExit + 1].GetComponent<Block>())
                {
                    yDestroy = yExit + 1;
                    xDestroy = xExit;
                }
            }
            else if (yExit == tilesPerRow - 1)
            {
                if (tiles[xExit, yExit - 1].GetComponent<Block>())
                {
                    yDestroy = yExit - 1;
                    xDestroy = xExit;
                }
            }
            else if (xExit == 0)
            {
                if (tiles[xExit + 1, yExit].GetComponent<Block>())
                {
                    yDestroy = yExit;
                    xDestroy = xExit + 1;
                }
            }
            else if (xExit == tilesPerRow - 1)
            {
                if (tiles[xExit - 1, yExit].GetComponent<Block>())
                {
                    yDestroy = yExit;
                    xDestroy = xExit - 1;
                }
            }

            if (!(yDestroy == -1 && xDestroy == -1))
            {
                GameObject alsoInTheWay = tiles[xDestroy, yDestroy];
                Transform anotherOldTransform = alsoInTheWay.transform;
                Destroy(alsoInTheWay);

                GameObject floor = Instantiate(FloorPrefab, anotherOldTransform.position, anotherOldTransform.rotation);
                floor.transform.localScale = TileScale;
                tiles[xDestroy, yDestroy] = floor;
            }

            _camController = GameObject.Find("Cameras").GetComponent<CameraController>();

            int index = rgen.Next(0, entityPlacementPossibilities.Count);
            GameObject go = tiles[entityPlacementPossibilities[index][0], entityPlacementPossibilities[index][1]];
            Player = Instantiate(PlayerPrefab, go.transform.position,
                PlayerPrefab.transform.rotation);
            Player.transform.localScale = TileScale;
            _camController.player = Player;
            Player.GetComponent<Player>().camController = _camController;
            entityPlacementPossibilities.RemoveAt(index);

            for (int i = 0; i < Constants.EnemyLimit; i++)
            {
                index = rgen.Next(0, entityPlacementPossibilities.Count);
                go = tiles[entityPlacementPossibilities[index][0], entityPlacementPossibilities[index][1]];
                GameObject newEnemy = Instantiate(SkullPrefab, go.transform.position,
                    SkullPrefab.transform.rotation);
                newEnemy.transform.localScale = TileScale;
                newEnemy.GetComponent<Skull>().OnCollisionEnter();
                Enemy.EnemyList.Add(newEnemy);
                entityPlacementPossibilities.RemoveAt(index);
            }
        }
    }
}