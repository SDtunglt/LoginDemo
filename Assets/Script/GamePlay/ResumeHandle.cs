using System;
using System.Collections;
using LuaFramework;
using UnityEngine;

public class ResumeHandle
{
    private GamePlayModel gameplayModel = GamePlayModel.Instance;
    
    private GiveCardsCompletedSignal giveCardsCompletedSignal = Signals.Get<GiveCardsCompletedSignal>();
    private ResumeCompletedSignal resumeCompletedSignal = Signals.Get<ResumeCompletedSignal>();
    
    private ResumeVO vo;
    
    public IEnumerator Execute(ResumeVO v)
    {
        vo = v;
        gameplayModel.resuming = true;
        yield return ResumeBoard();
        resumeCompletedSignal.Dispatch();
        gameplayModel.resuming = false;
    }
    
    private IEnumerator ResumeBoard()
    {
        try
        {
            var chiaBaiHandle = new ChiaBaiHandle();
            chiaBaiHandle.Execute(vo.chiaBaiVO, true);
            giveCardsCompletedSignal.Dispatch();
        }
        catch (Exception e)
        {
            Debug.Log($"Error ResumeBoard: {e}");
            FirebaseAnalyticsExtension.Instance.LogEventWithParam("ResumeBoard", "ResumeBoard", e.Message);
            gameplayModel.resuming = false;
            throw;
        }
        yield return IResumeGamePlay();
    }

    private IEnumerator IResumeGamePlay()
    {
        yield return null;
        if (vo.bocCaiVO == null) yield break;
        gameplayModel.status = BoardStatus.PLAYING;
        PlayModel.Instance.acts.Clear();
        if(vo.acts == null)
            yield break;
        try
        {
            foreach (var act in vo.acts)
            {
                var playHandle = new PlayHandle();
                playHandle.Execute(act);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error Resume PlayHandle: {e}");
            FirebaseAnalyticsExtension.Instance.LogEventWithParam("ResumePlayHandle", "ResumePlayHandle", e.Message);
            gameplayModel.resuming = false;
            throw;
        }
        //Nếu resume thì trước đó thằng này phải đã leave rồi
        //& event leave ấy đã đc gửi đến cho làng. Lúc đó, làng đã gọi timeOut rồi
        //khi làng gọi timeOut cho thằng này (lúc thấy nó leave) thì tại ClockTimeOutCommand
        //cả làng đã gọi PlayLogic.sendPlay, tại đó đã có 1 người take care ngay lập tức gửi play action rồi
        //=> khi resume không cần check đang ở lượt mình để gọi timeOut nữa
        if(vo.uvos == null)
            yield break;
        foreach (var uvo in vo.uvos)
        {
            var uHandel = new UHandle();
            uHandel.Execute(uvo);
        }
    }
}