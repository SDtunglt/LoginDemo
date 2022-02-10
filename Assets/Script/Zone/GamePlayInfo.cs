public class GamePlayInfo
{
    public int z;
    public int r;
    public int b;
    
    public string BoardName{
        get
        {
            var zs = GameConfig.ZoneCfg[z];
            if(z >= GameConfig.NormalZoneCount){
                return  zs.name + "/ Bàn " + (b + 1);
            }
            return zs.name + "/ " + zs.rooms[r] + "/ Bàn " + (b + 1);
        }
    }
    public string boardId{
        get { return z + "_" + r + "_" + b; }
    }
    public static GamePlayInfo fromBoardName(string bn){
        if (string.IsNullOrEmpty(bn)) return null;
        var arrBn = bn.Split('_');
        GamePlayInfo vo = new GamePlayInfo();
        vo.z = int.Parse(arrBn[0]);
        vo.r = int.Parse(arrBn[1]);
        vo.b = int.Parse(arrBn[2]);
        return vo;
    }
}