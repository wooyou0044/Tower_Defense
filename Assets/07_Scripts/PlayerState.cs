using UnityEngine;

public class PlayerState
{
    public int _playerLvl;
    public int _mapRerollCoin;
    public int _baseHP;
    public int _charLvl;

    public PlayerState(int playerLvl, int charLvl = 0)
    {
        _playerLvl = playerLvl;
        _mapRerollCoin = 25;

        float charLvlCal = 1 + (charLvl * 0.05f);
        _baseHP = Mathf.FloorToInt(1500 * charLvlCal);
    }
}
