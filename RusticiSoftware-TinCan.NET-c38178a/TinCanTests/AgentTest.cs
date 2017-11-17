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
    Copyright 2014 Rustici Software

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
    class AgentTest
    {
        [Test]
        public void TestEmptyCtr()
        {
            var obj = new Agent();
            Assert.IsInstanceOf<Agent>(obj);
            Assert.IsNull(obj.mbox);

            StringAssert.AreEqualIgnoringCase("{\"objectType\":\"Agent\"}", obj.ToJSON());
        }

        [Test]
        public void TestJObjectCtr()
        {
            var mbox = "mailto:tincancsharp@tincanapi.com";

            var cfg = new JObject();
            cfg.Add("mbox", mbox);

            var obj = new Agent(cfg);
            Assert.IsInstanceOf<Agent>(obj);
            Assert.That(obj.mbox, Is.EqualTo(mbox));
        }

        [Test]
        public void TestStringOfJSONCtr()
        {
            var mbox = "mailto:tincancsharp@tincanapi.com";

            var json = "{\"mbox\":\"" + mbox + "\"}";
            var strOfJson = new StringOfJSON(json);

            var obj = new Agent(strOfJson);
            Assert.IsInstanceOf<Agent>(obj);
            Assert.That(obj.mbox, Is.EqualTo(mbox));
        }
    }
}
