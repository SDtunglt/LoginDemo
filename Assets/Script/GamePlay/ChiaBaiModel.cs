
    public class ChiaBaiModel : Singleton<ChiaBaiModel>
    {
        /**update ở CardGivingMediator#onReceivedChonNoc với cả thằng bốc cái & những thằng còn lại*/
        public int nocIdx;
        /**update ở CardGivingMediator#onReceivedGiveCards với cả thằng bốc cái & những thằng còn lại*/
        public int baiCaiIdx;
        /**update ở ReceivedChiaBaiCommand*/
        public int cai;
        /**update ở ReceivedChiaBaiCommand*/
        public int playerHaveCaiIdx;
    }
