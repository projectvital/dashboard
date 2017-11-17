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

/*
    Copyright 2015 Rustici Software

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/
namespace TinCanTests
{
    using System;
    using NUnit.Framework;
    using Newtonsoft.Json.Linq;
    using TinCan;
    using TinCan.Json;

    [TestFixture]
    class ResultTest
    {
        [Test]
        public void TestEmptyCtr()
        {
            var obj = new Result();
            Assert.IsInstanceOf<Result>(obj);
            Assert.IsNull(obj.completion);
            Assert.IsNull(obj.success);
            Assert.IsNull(obj.response);
            Assert.IsNull(obj.duration);
            Assert.IsNull(obj.score);
            Assert.IsNull(obj.extensions);

            StringAssert.AreEqualIgnoringCase("{}", obj.ToJSON());
        }

        [Test]
        public void TestJObjectCtr()
        {
            var cfg = new JObject();
            cfg.Add("completion", true);
            cfg.Add("success", true);
            cfg.Add("response", "Yes");

            var obj = new Result(cfg);
            Assert.IsInstanceOf<Result>(obj);
            Assert.That(obj.completion, Is.EqualTo(true));
            Assert.That(obj.success, Is.EqualTo(true));
            Assert.That(obj.response, Is.EqualTo("Yes"));
        }

        [Test]
        public void TestStringOfJSONCtr()
        {
            var json = "{\"success\": true, \"completion\": true, \"response\": \"Yes\"}";
            var strOfJson = new StringOfJSON(json);

            var obj = new Result(strOfJson);
            Assert.IsInstanceOf<Result>(obj);
            Assert.That(obj.success, Is.EqualTo(true));
            Assert.That(obj.completion, Is.EqualTo(true));
            Assert.That(obj.response, Is.EqualTo("Yes"));
        }
    }
}
