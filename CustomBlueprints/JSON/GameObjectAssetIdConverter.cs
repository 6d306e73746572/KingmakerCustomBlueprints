﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace CustomBlueprints
{
    public class GameObjectAssetIdConverter : JsonConverter
    {

        public GameObjectAssetIdConverter() { }

        public override void WriteJson(JsonWriter w, object o, JsonSerializer szr)
        {

            var go = (UnityEngine.Object)o;
            var j = new JObject {
                {"$type", go.GetType().Name},
                {"InstanceId", go.GetInstanceID()},
                {"name", go.name },
            };
            j.WriteTo(w);
        }

        public override object ReadJson(JsonReader reader, Type type, object existing, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            JObject jObject = JObject.Load(reader);
            int instanceId = (int)jObject["InstanceId"];
            var result = BlueprintUtil.FindObjectByInstanceId(instanceId, type);
            if (result == null)
            {
                throw new System.Exception($"Couldn't find object with InstanceId {instanceId}");
            }
            return result;
        }
        public override bool CanConvert(Type type)
        {
            return typeof(GameObject).IsAssignableFrom(type) ||
                typeof(Transform).IsAssignableFrom(type);
        }
    }
}