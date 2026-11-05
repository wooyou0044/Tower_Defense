using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum eDefTowerType
{
    Canon,
    Radar,
}

//                          원형사거리 - 직사각형사거리 - 방향성사거리
public enum eAttAreaType { CircleType, RectType, SectorType }

[CreateAssetMenu(fileName = "NewDefTowerData", menuName = "DefTowerData", order = 1)]
public class DefTower : ScriptableObject
{
    //private int towerID;
    public int towerID;
    public string towerName;
    public int buildCoin;
    public int towerCount;
    public int att;
    public Vector2Int anchorOffset = new Vector2Int(0, 0);
    public eDefTowerType defType;
    public eAttAreaType attAreaType;
    public Vector2 attRangeRad;
    public float attSectorAngle;
    public GameObject defTowerObject;
    public Sprite defTowerImage;

    //public int TowerID
    //{
    //    get => towerID;
    //    set => towerID = value;
    //}

//    public int getTowerID => towerID;
//    public void OnValidate()
//    {
//#if UNITY_EDITOR
//        if(towerID == 0)
//        {
//            string path = AssetDatabase.GetAssetPath(this);
//            string guid = AssetDatabase.AssetPathToGUID(path);

//            if(!string.IsNullOrEmpty(guid))
//            {
//                int hash = Mathf.Abs(guid.GetHashCode());
//                towerID = 1000 + (hash % 9000);
//                EditorUtility.SetDirty(this);
//            }
//        }
//#endif
//    }
}
