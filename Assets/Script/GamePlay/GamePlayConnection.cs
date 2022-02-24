using System;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GamePlayConnection : ConnectionCpnt
{
    private GamePlayLogic gamePlayLogic = GamePlayLogic.Instance;
    private GamePlayModel gamePlayModel = GamePlayModel.Instance;
}
