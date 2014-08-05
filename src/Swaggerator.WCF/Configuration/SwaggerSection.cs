/*
 * Copyright (c) 2014 Digimarc Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * SwaggerSection.cs : Swaggerator configuration section model.
 */

using System.Configuration;

namespace Swaggerator.Configuration
{
	public class SwaggerSection : ConfigurationSection
	{
		[ConfigurationProperty("tags", IsRequired = true)]
		public TagCollection Tags
		{
			get { return (TagCollection)this["tags"]; }
			set { this["tags"] = value; }
		}

		[ConfigurationProperty("settings", IsRequired = false)]
		public SettingCollection Settings
		{
			get { return (SettingCollection) this["settings"]; }
			set { this["settings"] = value; }
		}
	}
}
