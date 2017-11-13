using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Objects.Types
{
    public enum CustomXAPIDefinitionVerbs
    {
        VoiceRecorded,
        PrintedToPdf,
        Navigated
    }
    
    public static class CustomXAPIDefinitionVerbsConverter
    {
        public static string Convert(CustomXAPIDefinitionVerbs value)
        {
            switch (value)
            {
                case CustomXAPIDefinitionVerbs.VoiceRecorded: return "verb/voice-recorded";
                case CustomXAPIDefinitionVerbs.PrintedToPdf:  return "verb/printed-to-pdf";
                case CustomXAPIDefinitionVerbs.Navigated: return "verb/navigated";
                default: return null;
            }
        }
    }

}
