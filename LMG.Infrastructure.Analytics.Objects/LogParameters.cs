/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using LMG.Infrastructure.Analytics.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMG.Infrastructure.Analytics.Objects
{
    public class LogParameters
    {
        public Uri AccessedUri { get; set; }
        public string AccessedContentTitle { get; set; }
        public string AccessedContentDescription { get; set; }

        public long? LearningModuleId { get; set; }
        public long? LearningModuleProfileId { get; set; }
        public long? TheoryPageId { get; set; }
        public long? ExerciseId { get; set; }
        public ExerciseTypes? ExerciseType { get; set; }
        public long? StudentId { get; set; }
        public long? TheoryPageTreeItemId { get; set; }
        public long? ExerciseTreeItemId { get; set; }
        public long? DictionaryId { get; set; }
        public string HintId { get; set; }
        public string SolutionId { get; set; }
        public string FeedbackId { get; set; }
        public string ExploreHotspotId { get; set; }
        public long? SentenceId { get; set; }

        public Uri ParentUri { get; set; }
        public string ParentTitle {get;set;}

        //Next properties come in through SOAP headers.
        public Guid? Registration { get; set; }
        public string InstanceId { get; set; }
    }

    public class LogParametersForAccess : LogParameters
    {
        /// <summary>
        /// Marks which of the provided content Ids was actually accessed.
        /// </summary>
        public AccessTypes AccessType { get; set; }        
        /// <summary>
        /// Marks which of the provided content Ids was actually accessed.
        /// </summary>
        public AccessTypes? ParentAccessType { get; set; }
        /// <summary>
        /// For linked pages. Add the link to the parent. E.g. the exercise URI which shows several linked theory pages at the bottom.
        /// </summary>
        public Uri LinkReferrer { get; set; }
        /// <summary>
        /// Title of the tab. This can be Intro, Begin, Content, End
        /// </summary>
        public string TabTitle { get; set; }

        public int? ExploreHotspotTabId { get; set; }
        public int? ExploreHotspotTabCount { get; set; }

        public NavigationTypes? NavigationType { get; set; }
        public Guid? SessionMigrationTo { get; set; }
    }

    public class LogParametersForExerciseResult : LogParameters
    {
        public double? Percentage { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Raw { get; set; }
        public int? Attempt { get; set; }
        public int? MaxAttempts { get; set; }
        public int? AmountCorrect { get; set; }
        public int? AmountWrong { get; set; }
        public string Response { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsSuccess { get; set; }
        public long? DurationTicks { get; set; }
        public Dictionary<string, string> UserAnswers { get; set; }
        public Dictionary<string, string> CorrectAnswers { get; set; }
    }
    public class LogParametersForExerciseInteraction : LogParametersForExerciseResult
    {
        public ExerciseInteractionTypes? ExerciseInteractionType { get; set; }
        public string MatchDragId { get; set; }
        public string MatchDropTargetId { get; set; }
    }
    public class LogParametersForMediaInteraction: LogParameters
    {
        public MediaInteractionTypes MediaInteractionType { get; set; }
        public float? MediaStartingPoint { get; set; }
        public float? MediaEndingPoint { get; set; }
        public bool? IsVoiceRecording { get; set; }
    }
    public class LogParametersForDictionaryEntrySearch : LogParameters
    {
        public long? DictionaryEntryId { get; set; }
        public string DictionarySearchTerm { get; set; }
        public string DictionarySearchOption { get; set; }
        public int? DictionarySearchResultCount { get; set; }
    }

}
