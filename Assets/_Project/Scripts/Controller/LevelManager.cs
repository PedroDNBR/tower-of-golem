using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelManager() { instance = this; }

    public GameObject bossArenaOusideWalls;
    public GameObject bossArenaInsideWalls;

    public GameObject bossArenaLeftDoor;
    public GameObject bossArenaRightDoor;
}
