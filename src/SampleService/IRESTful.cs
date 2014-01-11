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
 * IRESTful.cs : Sample service declaration showing multiple methods, with summary and notes attributes
 */

using System;
using Swaggerator.Attributes;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace SampleService
{
	[ServiceContract]
	public interface IRESTful
	{
		//[OperationContract]
		//[WebGet(UriTemplate = "/Data/{value}")]
		//string GetData(string value);

		//[OperationSummary("Does stuff.")]
		//[OperationNotes("I mean, it does some really interesting stuff. Stuff like you wouldn't believe.")]
		//[ResponseCode(400, "Four hundred error")]
		//[ResponseCode(200, "OK")]
		//[ResponseCode(205, "Some error")]
		//[ResponseCode(404, "Not found")]
		//[ResponseCode(401, "Something weird happened")]
		//[ResponseCode(301, "Three O one Something weird happened")]
		//[OperationContract]
		//[WebInvoke(UriTemplate = "/data", Method = "POST", RequestFormat = WebMessageFormat.Json,
		//	BodyStyle = WebMessageBodyStyle.Bare)]
		//CompositeType GetDataUsingDataContract(CompositeType composite);

		//[OperationContract]
		//[WebInvoke(
		//	UriTemplate = "/Data/{value}?val={anothervalue}&option={optionalvalue}",
		//	Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
		//	BodyStyle = WebMessageBodyStyle.Bare)]
		//string PutData(string value,
		//	[ParameterSettings(IsRequired = true, Description = "Yes, you need to include this.")] string anothervalue,
		//	[ParameterSettings(UnderlyingType = typeof (int))] string optionalvalue, 
		//	[ParameterSettings(TypeSizeNote = "123")] string valueWithLengthRequirement);

		//[OperationContract]
		//[Swaggerator.Attributes.Produces(ContentType = "application/xml")]
		//[Swaggerator.Attributes.Produces(ContentType = "application/customformat")]
		//[WebGet(UriTemplate = "/List")]
		//CompositeType[] GetList();

		//[OperationContract]
		//[WebInvoke(UriTemplate = "/Data/{value}",
		//	Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
		//	BodyStyle = WebMessageBodyStyle.Bare)]
		//void Delete(string value);

		//[OperationContract]
		//[WebGet(UriTemplate = "/Data2/{value}")]
		//[OperationSummary("Example for hiding a parameter")]
		//[OperationNotes("The second parameter, object 'bar' is hidden")]
		//int HideOneOfTwoParams(int value, [ParameterSettings(Hidden = true)]object bar);


		//[WebGet(UriTemplate = "/Data2Asynch/{value}")]
		//[OperationSummary("Example for hiding parameters and overriding return type")]
		//[OperationNotes("Two parameters, AsynchCallback callback and object asyncState are hidden")]
		//[OverrideReturnType(typeof(CompositeType))]
		//[OperationContractAttribute(AsyncPattern = true)]
		//IAsyncResult BeginServiceAsyncMethod(string value, [ParameterSettings(Hidden = true)]AsyncCallback callback, [ParameterSettings(Hidden = true)]object asyncState);

		//// Note: There is no OperationContractAttribute for the end method.
		//CompositeType EndServiceAsyncMethod(IAsyncResult result);

		[OperationContract]
		[WebGet(UriTemplate = "/DisplayContractName")]
		CustomDataContractSample DisplayDataContractNameInsteadOfClassName();
	}



}
