/*
 * Copyright (c) 2013 Digimarc Corporation
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
 * IRESTful.cs : Sample service declaration showing multiple methods, with summary and notes attributes
 */

using Swaggerator.Attributes;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using System.Collections.Generic;

namespace SampleService
{
	[ServiceContract]
	public interface IRESTful
	{
		[OperationContract]
		[WebGet(UriTemplate = "/Data/{value}")]
		string GetData(string value);

		[OperationSummary("Does stuff.")]
		[OperationNotes("I mean, it does some really interesting stuff. Stuff like you wouldn't believe.")]
		[ResponseCode(400, "This will be overridden by the implementation")]
		[OperationContract]
		[WebInvoke(UriTemplate = "/data", Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		CompositeType GetDataUsingDataContract(CompositeType composite);

		[OperationContract]
		[WebInvoke(
			UriTemplate = "/Data/{value}?val={anothervalue}&option={optionalvalue}",
			Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		string PutData(string value, [ParameterSettings(IsRequired = true, Description = "Yes, you need to include this.")]string anothervalue,
			[ParameterSettings(UnderlyingType = typeof(int))]string optionalvalue);

		[OperationContract]
		[WebGet(UriTemplate = "/List")]
		CompositeType[] GetList();
	}



}
