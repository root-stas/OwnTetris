public class TFigure {

    public byte BlocksCount = 2;
    public byte[,] _nodeBlock = new byte[4,4];
    public byte[,] _newBlock = new byte[4,4];

    private System.Random _rnd = new System.Random();

	public void CreateBlock () {
        for (int _v = 0; _v < 4; _v++)
            for (int _h = 0; _h < 4; _h++)
            {
                _nodeBlock[_v, _h] = _newBlock[_v, _h];
                _newBlock[_v, _h] = 0;
            }

        _newBlock[1, 1] = (byte)_rnd.Next(1, BlocksCount + 1);
        _newBlock[2, 1] = (byte)_rnd.Next(1, BlocksCount + 1);
        int _verPos = _rnd.Next(0, 4);
        int _horPos = _rnd.Next(0, 3);
        int _inserted = 2;

        while (_inserted < 4)
        {
            if (_verPos == 0 || _verPos == 3) _horPos = 1;
            if (_newBlock[_verPos, _horPos] == 0)
            {
                _newBlock[_verPos, _horPos] = (byte)_rnd.Next(1, BlocksCount + 1);
                _inserted++;
            }
            else
            {
                _verPos = _rnd.Next(0, 4);
                _horPos = _rnd.Next(0, 3);
            }
        }
    }

    public void TurnBlockClock()
    {
        int _size = _nodeBlock.GetLength(0);
        for (int _v = 0; _v < _size / 2; _v++)
            for (int _h = _v; _h < _size - _v - 1; _h++)
            {
                byte _value = _nodeBlock[_v, _h];
                _nodeBlock[_v, _h] = _nodeBlock[_size - _h - 1, _v];
                _nodeBlock[_size - _h - 1, _v] = _nodeBlock[_size - _v - 1, _size - _h - 1];
                _nodeBlock[_size - _v - 1, _size - _h - 1] = _nodeBlock[_h, _size - _v - 1];
                _nodeBlock[_h, _size - _v - 1] = _value;
            }
    }

}