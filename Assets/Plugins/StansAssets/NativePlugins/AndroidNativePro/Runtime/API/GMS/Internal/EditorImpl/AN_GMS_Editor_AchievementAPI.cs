using System;
using SA.Foundation.Templates;
using SA.Android.Content;
using SA.Android.GMS.Games;
using StansAssets.Foundation.Async;

namespace SA.Android.GMS.Internal
{
    class AN_GMS_Editor_AchievementAPI : AN_iGMS_AchievementsAPI
    {
        public void Unlock(AN_AchievementsClient client, string achievementId) { }

        public void UnlockImmediate(AN_AchievementsClient client, string achievementId, Action<SA_Result> callback)
        {
             CoroutineUtility.WaitForSeconds(1, () =>
            {
                callback.Invoke(new SA_Result());
            });
        }

        public void Increment(AN_AchievementsClient client, string achievementId, int step) { }

        public void IncrementImmediate(AN_AchievementsClient client, string achievementId, int step, Action<AN_AchievementIncrementResult> callback)
        {
             CoroutineUtility.WaitForSeconds(1, () =>
            {
                callback.Invoke(new AN_AchievementIncrementResult());
            });
        }

        public void Reveal(AN_AchievementsClient client, string achievementId) { }

        public void RevealImmediate(AN_AchievementsClient client, string achievementId, Action<SA_Result> callback)
        {
             CoroutineUtility.WaitForSeconds(1, () =>
            {
                callback.Invoke(new SA_Result());
            });
        }

        public void GetAchievementsIntent(AN_AchievementsClient client, Action<AN_IntentResult> callback)
        {
             CoroutineUtility.WaitForSeconds(1, () =>
            {
                callback.Invoke(new AN_IntentResult(new AN_Intent()));
            });
        }

        public void Load(AN_AchievementsClient client, bool forceReload, Action<AN_AchievementsLoadResult> callback)
        {
             CoroutineUtility.WaitForSeconds(1, () =>
            {
                //TODO might me cool to get info from saved games-ids.xml
                callback.Invoke(new AN_AchievementsLoadResult());
            });
        }
    }
}
