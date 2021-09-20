// <auto-generated/>

//------------------------------------------------------------------------------------------------
//      This file has been programatically generated; DON´T EDIT!
//------------------------------------------------------------------------------------------------

#pragma warning disable SA1001
#pragma warning disable SA1027
#pragma warning disable SA1028
#pragma warning disable SA1121
#pragma warning disable SA1205
#pragma warning disable SA1309
#pragma warning disable SA1402
#pragma warning disable SA1505
#pragma warning disable SA1507
#pragma warning disable SA1508
#pragma warning disable SA1652

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Text.Json;

namespace SharpGLTF.Schema2
{
	using Collections;

	/// <summary>
	/// glTF Extension for an individual node in a glTF model, to associate it with the model's root AGI_articulations object.
	/// </summary>
	partial class AgiNodeArticulations : ExtraProperties
	{
	
		private String _articulationName;
		
		private Boolean? _isAttachPoint;
		
	
		protected override void SerializeProperties(Utf8JsonWriter writer)
		{
			base.SerializeProperties(writer);
			SerializeProperty(writer, "articulationName", _articulationName);
			SerializeProperty(writer, "isAttachPoint", _isAttachPoint);
		}
	
		protected override void DeserializeProperty(string jsonPropertyName, ref Utf8JsonReader reader)
		{
			switch (jsonPropertyName)
			{
				case "articulationName": _articulationName = DeserializePropertyValue<String>(ref reader); break;
				case "isAttachPoint": _isAttachPoint = DeserializePropertyValue<Boolean?>(ref reader); break;
				default: base.DeserializeProperty(jsonPropertyName,ref reader); break;
			}
		}
	
	}

}
