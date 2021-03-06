﻿//
//  ExamplePortalConfigTests.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017-2020 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.IO;
using R7.News.Components;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace R7.News.Tests
{
    public class DevelopmentPortalConfigTests
    {
        [Fact]
        public void PortalConfigExistsTest ()
        {
            Assert.Equal (true, File.Exists (GetDevelopmentConfigFile ()));
        }

        [Fact]
        public void PortalConfigDeserializationTest ()
        {
            var config = DeserializeConfig (GetDevelopmentConfigFile ());
            Assert.NotNull (config);
        }

        [Fact]
        public void PortalConfigFieldLengthTest ()
        {
            var config = DeserializeConfig (GetDevelopmentConfigFile ());

            foreach (var discussProvider in config.DiscussProviders) {
                Assert.True (discussProvider.ProviderKey.Length <= 64);
            }
        }

        protected string GetDevelopmentConfigFile ()
        {
            return Path.Combine ("..", "..", "..", "R7.News", "R7.News.development.yml");
        }

        protected NewsPortalConfig DeserializeConfig (string configFile)
        {
            using (var configReader = new StringReader (File.ReadAllText (configFile))) {
                var deserializer = new DeserializerBuilder ().WithNamingConvention (HyphenatedNamingConvention.Instance).Build ();
                return deserializer.Deserialize<NewsPortalConfig> (configReader);
            }
        }
    }
}
