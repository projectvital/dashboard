using LMG.Infrastructure.Analytics.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMG.Infrastructure.Analytics.Objects.Helpers
{
    public static class LRSLexiconHelper
    {
        public static TinCan.Verb GetVerb(Verbs verb)
        {
            #region Get details from DB
            Uri uri = new Uri("http://adlnet.gov/expapi/verbs/completed");

            TinCan.Verb rverb = new TinCan.Verb(uri);
            rverb.display = new TinCan.LanguageMap();
            rverb.display.Add("nl", "voltooid");
            rverb.display.Add("en", "completed");
            #endregion

            return rverb;
        }
    }
}
