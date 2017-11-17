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
using TinCan;
using LMG.Infrastructure.Analytics.Objects.Helpers;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Extensions
{
    public static class StatementExtension
    {
        #region Verb
        public static Statement SetVerb(this Statement statement, Verbs verb)
        {
            if (statement == null)
                return null;

            Verb selected_verb = LRSLexiconHelper.GetVerb(verb);
            statement.verb = selected_verb;
            return statement;
        }
        #endregion

        #region Actor
        private static void EnsureExistenceOfContainer_Actor(Statement statement)
        {
            if (statement.actor == null)
                statement.actor = new Agent();
        }
        
        public static Statement SetActor(this Statement statement, object some_identifier)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Actor(statement);

            //statement.actor.
            return statement;
        }
        #endregion

        #region Result
        private static void EnsureExistenceOfContainer_Result(Statement statement)
        {
            if (statement.result == null)
                statement.result = new Result();
        }
        
        public static Statement SetResult_ScorePercentage(this Statement statement, double percentage)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Result(statement);
            statement.result.SetScorePercentage(percentage);
            return statement;
        }
        public static Statement SetResult_Success(this Statement statement, bool success)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Result(statement);
            statement.result.SetSuccess(success);
            return statement;
        }
        public static Statement SetResult_Response(this Statement statement, string response)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Result(statement);
            statement.result.SetResponse(response);
            return statement;
        }
        public static Statement SetResult_Completion(this Statement statement, bool complete)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Result(statement);
            statement.result.SetCompletion(complete);
            return statement;
        }
        public static Statement SetResult_Duration(this Statement statement, TimeSpan duration)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Result(statement);
            statement.result.SetDuration(duration);
            return statement;
        }
        #endregion

        #region Context
        private static void EnsureExistenceOfContainer_Context(Statement statement)
        {
            if (statement.context == null)
                statement.context = new Context();
        }

        public static Statement SetContext_Field_Stub(this Statement statement, string response)
        {
            if (statement == null)
                return null;

            EnsureExistenceOfContainer_Context(statement);

            //statement.context.;
            return statement;
        }
        #endregion
    }
}
