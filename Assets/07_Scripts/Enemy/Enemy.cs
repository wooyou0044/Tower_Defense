public enum enemyType
{
    Dummy,
    end_Num
}

public class Enemy
{
    public int _enemyHP;
    private int _enemyAtt;
    enemyType _enemyType;

    public Enemy()
    {
        // 일단 공격력 기본으로 25로 해놓고 기본 적들끼리랑 보스 적이랑 구분 짓기
        _enemyAtt = 25;
    }

    public int GetEnemyAtt()
    {
        return _enemyAtt;
    }
}
