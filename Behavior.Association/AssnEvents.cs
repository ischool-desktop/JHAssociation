using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//此事件當使用者按下重新整理時
//會引發社團資訊的更新*註1
//目的是學生狀態更新時,需要一併更新相關資料
//但是又不希望因為事件而引發太多地方的更新事件
//造成系統被拖慢

//註1:
//社團主清單資訊更新
//修課學生清單更新
//教師資訊更新
//評量資訊更新

namespace JHSchool.Association
{
    public static class AssnEvents
    {
        public static void RaiseAssnChanged()
        {
            if (AssnChanged != null)
                AssnChanged(null, EventArgs.Empty);
        }

        public static event EventHandler AssnChanged;
    }
}