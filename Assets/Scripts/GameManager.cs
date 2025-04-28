using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool inGame;

    private GameObject _mainMenuScreen;
    private GameObject _gameOverScreen;
    private GameObject _gameClearScreen;
    private GameObject _inGameScreen;

    private GameObject _walls;
    private PlayerController _player;

    private void Start()
    {
        inGame = false;
        _walls = GameObject.Find("Walls");
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _walls.SetActive(false);
        _mainMenuScreen = transform.Find("StartMenuScreen").gameObject;
        _gameOverScreen = transform.Find("GameOverScreen").gameObject;
        _gameClearScreen = transform.Find("GameClearScreen").gameObject;
        _inGameScreen = transform.Find("InGameScreen").gameObject;
        _mainMenuScreen.SetActive(true);
        _gameOverScreen.SetActive(false);
        _gameClearScreen.SetActive(false);
        _inGameScreen.SetActive(false);
    }

    public void GameStart()
    {
        inGame = true;
        _mainMenuScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
        _gameClearScreen.SetActive(false);
        _inGameScreen.SetActive(true);
        _walls.SetActive(true);
        _player.Heal(_player.maxHealth);
        _player.transform.position = new Vector3(0, -2, -2);
        
    }

    public void GameOver()
    {
        inGame = false;
        _mainMenuScreen.SetActive(false);
        _gameOverScreen.SetActive(true);
        _gameClearScreen.SetActive(false);
        _inGameScreen.SetActive(false);
        _walls.SetActive(false);
    }

    public void GameClear()
    {
        inGame = false;
        _mainMenuScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
        _gameClearScreen.SetActive(true);
        _inGameScreen.SetActive(false);
        _walls.SetActive(false);
    }
}
