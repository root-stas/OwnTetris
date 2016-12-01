public class TetrisEngine {

    public byte[,] GameGrid = new byte[20, 10];
    public TFigure GameFigure;
    public int _horPos, _verPos;
    public int Score = 0;
    public int _stepTime = 50;
    public byte _move = 0;
    public bool _turn = false;
    public int _rowFX = -1;

    private bool _addBlock = true;
    private float _lastTime = 10;
    private System.Random _rnd = new System.Random();

    public void InitGame()
    {
        GameFigure = new TFigure();
    }

    bool CheckLeft()
    {
        for (int _h = 0; _h < 2; _h++)
            for (int _v = 0; _v < 4; _v++)
            {
                if (GameFigure._nodeBlock[_v, _h] != 0 && _horPos + _h < 1) return false;
                if (_verPos + _v >= 0 && _horPos + _h >= 0 && _verPos + _v < GameGrid.GetLength(0) && _horPos + _h < GameGrid.GetLength(1))
                    if (GameGrid[_verPos + _v, _horPos + _h] != 0) return false;
            }
        return true;
    }

    bool CheckRight()
    {
        for (int _h = 3; _h > 0; _h--)
            for (int _v = 0; _v < 4; _v++)
            {
                if (GameFigure._nodeBlock[_v, _h] != 0 && _horPos + _h > GameGrid.GetLength(1) - 2) return false;
                if (_verPos + _v >= 0 && _horPos + _h >= 0 && _verPos + _v < GameGrid.GetLength(0) && _horPos + _h < GameGrid.GetLength(1))
                    if (GameGrid[_verPos + _v, _horPos + _h] != 0) return false;
            }
        return true;
    }

    bool CheckDawn()
    {
        for (int _v = 3; _v >= 0; _v--)
            for (int _h = 0; _h < 4; _h++)
                if (GameFigure._nodeBlock[_v, _h] != 0 && _verPos + _v >= 0 && _horPos + _h >= 0 && _horPos + _h < GameGrid.GetLength(1))
                    if (GameGrid[_verPos + _v, _horPos + _h] > 0)
                        return false;
                    else if (_verPos + _v >= GameGrid.GetLength(0) - 1)
                    {
                        _verPos++;
                        return false;
                    }
        return true;
    }

    public void NewGame()
    {
        for (int _v = 0; _v < GameGrid.GetLength(0); _v++)
            for (int _h = 0; _h < GameGrid.GetLength(1); _h++)
                GameGrid[_v, _h] = 0;
        Score = 0;
        _stepTime = 74;
        _addBlock = true;
    }

    void DellRow()
    {
        int _blocks;
        for (int _v = 0; _v < GameGrid.GetLength(0); _v++)
        {
            _blocks = 0;
            for (int _h = 0; _h < GameGrid.GetLength(1); _h++)
                if (GameGrid[_v, _h] != 0) _blocks++;

            if (_blocks == GameGrid.GetLength(1))
            {
                if (_stepTime > 6) _stepTime -= 5;
                _rowFX = _v;
                for (int _h = 0; _h < GameGrid.GetLength(1); _h++)
                {
                    Score += 100;

                    for (int _line = _v - 1; _line >= 0; _line--)
                        GameGrid[_line + 1, _h] = GameGrid[_line, _h];
                }
            }
        }
    }

    public void Update(float _time)
    {
        if (_addBlock)
        {
            GameFigure.CreateBlock();
            if (GameFigure._nodeBlock[2, 1] == 0) GameFigure.CreateBlock();
            _horPos = _rnd.Next(0, GameGrid.GetLength(1) - 3);
            _verPos = -4;
            _lastTime = _time;
            _addBlock = false;
        }

        if (_lastTime + _stepTime / 100f < _time)
        {
            if (CheckDawn())
            {
                Score++;
                _verPos++;
                if (_move == 1 && CheckRight()) _horPos++;
                if (_move == 2 && CheckLeft()) _horPos--;
                if (_turn && CheckLeft() && CheckRight()) GameFigure.TurnBlockClock();
                _move = 0;
                _turn = false;
            }

            if (!CheckDawn())
            {
                _addBlock = true;
                _verPos--;
                if (_verPos < 0)
                    NewGame();
                else
                {
                    for (int _v = 0; _v < 4; _v++)
                        for (int _h = 0; _h < 4; _h++)
                            if (GameFigure._nodeBlock[_v, _h] != 0) GameGrid[_verPos + _v, _horPos + _h] = GameFigure._nodeBlock[_v, _h];
                    DellRow();
                }
            }

            _lastTime = _time;
        }
    }

}