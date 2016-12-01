using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main : MonoBehaviour {
    public Material[] Colors;
    public Material[] Numbers;
    public GameObject BlockSample;
    public Text BestScoreTxT;
    public ParticleSystem RowFX;

    private Renderer[,] Grid;
    private Renderer[,] NextBlock;
    private Renderer[] ScoreNumber;
    private Renderer[] LevelNumber;
    private string _score;
    private TetrisEngine Tetris;
    private int BestScore;
    private bool InGame = false;

    // Use this for initialization
    void Start () {
        //Load saved score:
        BestScore = PlayerPrefs.GetInt("BestScore", 0);
        BestScoreTxT.text = "BEST SCORE: " + BestScore.ToString();
        Grid = new Renderer[20, 10];
        //Init graphics object:
        NextBlock = new Renderer[4, 4];
        ScoreNumber = new Renderer[5];
        LevelNumber = new Renderer[2];
        GameObject _block;
        for (int _number = 0; _number < ScoreNumber.Length; _number++)
        {
            _block = (GameObject)Instantiate(BlockSample, new Vector3(7 - _number * 2, 22.75f, 0), Quaternion.identity);
            _block.transform.localScale = Vector3.one * 1.8f;
            ScoreNumber[_number] = _block.GetComponent<Renderer>();
            ScoreNumber[_number].material = Numbers[0];
            _block.transform.SetParent(transform);
        }
        for (int _number = 0; _number < LevelNumber.Length; _number++)
        {
            _block = (GameObject)Instantiate(BlockSample, new Vector3(-2 - _number, 13.4f, 0), Quaternion.identity);
            LevelNumber[_number] = _block.GetComponent<Renderer>();
            LevelNumber[_number].material = Numbers[0];
            _block.transform.SetParent(transform);
        }
        for (int _v = 0; _v < Grid.GetLength(0); _v++)
            for (int _h = 0; _h < Grid.GetLength(1); _h++)
            {
                _block = (GameObject)Instantiate(BlockSample, new Vector3(_h, 20 - _v, 0), Quaternion.identity);
                _block.transform.SetParent(transform);
                Grid[_v, _h] = _block.GetComponent<Renderer>();
                if (_v < 4 && _h < 4)
                {
                    _block = (GameObject)Instantiate(BlockSample, new Vector3(_h - 4, 19 - _v, 0), Quaternion.identity);
                    _block.transform.SetParent(transform);
                    NextBlock[_v, _h] = _block.GetComponent<Renderer>();
                }
            }
        //Init tetris game:
        Tetris = new TetrisEngine();
        Tetris.InitGame();
        Tetris.GameFigure.BlocksCount = (byte)(Colors.Length - 1);

        StartCoroutine(UpdateScreen());
        //StartCoroutine(FPS());
    }
    //Draw FX if colected row:
    IEnumerator UpdateScreen()
    {
        while (true)
        {
            if (Tetris._rowFX > -1)
            {
                RowFX.transform.position = new Vector3(4.5f, Tetris.GameGrid.GetLength(0) - Tetris._rowFX, 1);
                RowFX.enableEmission = false;
                RowFX.Play();
                RowFX.enableEmission = true;
                Debug.Log("FX " + Tetris._rowFX);
                Tetris._rowFX = -1;
            }

            yield return new WaitForSeconds(0.06f);
        }
    }

    private int _FPS = 0;
    IEnumerator FPS()
    {
        while (true)
        {
            _FPS = 0;
            yield return new WaitForSeconds(1f);
        }
    }
    //New game button:
    public void NewGame()
    {
        InGame = true;
        BestScoreTxT.transform.parent.parent.gameObject.SetActive(false);
    }

    private float _touchPos;
    // Update is called once per frame
    void Update () {
        //_FPS++;
        if (Input.GetAxis("Cancel") > 0.1f)
        {
            if (InGame)
            {
                InGame = false;
                BestScoreTxT.transform.parent.parent.gameObject.SetActive(true);
                Tetris.NewGame();
            }
            else Application.Quit();
        }
        if (InGame)
        {
            //GamePad:
            if (Input.GetAxis("Fire2") != 0f) Tetris._turn = true;
            if (Input.GetAxis("Horizontal") > 0.2f) { Tetris._move = 1; Input.ResetInputAxes(); }
            if (Input.GetAxis("Horizontal") < -0.2f) { Tetris._move = 2; Input.ResetInputAxes(); }
            //Mouse and touch:
            if (Input.GetMouseButtonDown(0)) _touchPos = Input.mousePosition.x;
            if (Input.GetMouseButtonUp(0))
            {
                _touchPos -= Input.mousePosition.x;
                if (_touchPos > 5) Tetris._move = 2;
                else if (_touchPos < -5) Tetris._move = 1;
                else Tetris._turn = true;
                _touchPos = 0;
            }
            //Update game state:
            Tetris.Update(Time.time);
            //Draw blocks:
            for (int _v = 0; _v < Tetris.GameGrid.GetLength(0); _v++)
                for (int _h = 0; _h < Tetris.GameGrid.GetLength(1); _h++)
                {
                    if (_v >= Tetris._verPos && _v < Tetris._verPos + 4 && _h >= Tetris._horPos && _h < Tetris._horPos + 4)
                    {
                        if (Tetris.GameFigure._nodeBlock[_v - Tetris._verPos, _h - Tetris._horPos] != 0)
                        {
                            Grid[_v, _h].gameObject.SetActive(true);
                            Grid[_v, _h].material = Colors[Tetris.GameFigure._nodeBlock[_v - Tetris._verPos, _h - Tetris._horPos]];
                        }
                        else
                        {

                            if (Tetris.GameGrid[_v, _h] == 0)
                                Grid[_v, _h].gameObject.SetActive(false);
                            else
                            {
                                Grid[_v, _h].gameObject.SetActive(true);
                                Grid[_v, _h].material = Colors[Tetris.GameGrid[_v, _h]];
                            }
                        }
                    }
                    else
                    {

                        if (Tetris.GameGrid[_v, _h] == 0)
                            Grid[_v, _h].gameObject.SetActive(false);
                        else
                        {
                            Grid[_v, _h].gameObject.SetActive(true);
                            Grid[_v, _h].material = Colors[Tetris.GameGrid[_v, _h]];
                        }
                    }

                    if (_v < 4 && _h < 4)
                    {
                        if (Tetris.GameFigure._newBlock[_v, _h] == 0)
                            NextBlock[_v, _h].gameObject.SetActive(false);
                        else
                        {
                            NextBlock[_v, _h].gameObject.SetActive(true);
                            NextBlock[_v, _h].material = Colors[Tetris.GameFigure._newBlock[_v, _h]];
                        }
                    }
                }
            //Draw score and speed:
            _score = Tetris.Score.ToString();
            if (Tetris.Score > BestScore)
            {
                BestScore = Tetris.Score;
                BestScoreTxT.text = "BEST SCORE: " + _score;
                PlayerPrefs.SetInt("BestScore", BestScore);
            }
            for (int _number = 0; _number < ScoreNumber.Length; _number++)
                if (_number < _score.Length)
                    ScoreNumber[_number].material = Numbers[int.Parse(_score[_score.Length - 1 - _number].ToString())];
                else
                    ScoreNumber[_number].material = Numbers[0];

            //_score = _FPS.ToString();
            _score = (75 - Tetris._stepTime).ToString();
            for (int _number = 0; _number < LevelNumber.Length; _number++)
                if (_number < _score.Length)
                    LevelNumber[_number].material = Numbers[int.Parse(_score[_score.Length - 1 - _number].ToString())];
                else
                    LevelNumber[_number].material = Numbers[0];
        }
    }
}