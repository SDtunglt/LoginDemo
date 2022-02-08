// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("H4jrgt9zZzM22P2h8+KEzX3LQ0/OFvGDdXm52EGWHgLVk2COyKbxckDTVEr6fq5EKckJU+6nc5qRXJCeZNZVdmRZUl1+0hzSo1lVVVVRVFcUb/kMCCpMHug3kT/9+Nmi1oFtedZVW1Rk1lVeVtZVVVTRSRAkts2AQXzkmAN2MkD6gPp7iWc5yQj+e1tu/Q494OmJvrvqZpI3ggb1LaA1vtdy21mFsLhrNeLM4cF5UGDuOgkFRcJOtbsy9x+/B6Y45uTSFJ/u5c+O6AVazhGEtrE4NWi1KD7QJtPfFV2+VIeEAOYyyb2OezgrW7qWMJyg+OsT74pC+/5A7jq1SKtwyOMGtAq0S7yo7zSN5r/7ehlqZhrir4YHf1/HUvS7woP6SVZXVVRV");
        private static int[] order = new int[] { 11,3,8,11,13,11,11,11,8,13,10,11,12,13,14 };
        private static int key = 84;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
