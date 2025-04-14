using System;
using System.Text.Json;

namespace Lab8Csharp.networking.protocol
{
    [Serializable]
    public class Response
    {
        public ResponseType Type { get; set; }  // ← public setter (pentru JSON deserialization)
        public object? Data { get; set; }

        public Response() { }

        
        public T GetData<T>()
        {
            return JsonSerializer.Deserialize<T>(Data.ToString());
        }

        public override string ToString()
        {
            return $"Response{{type='{Type}', data='{Data}'}}";
        }

        public class Builder
        {
            private readonly Response _response = new Response();

            public Builder Type(ResponseType type)
            {
                _response.Type = type;
                return this;
            }

            public Builder Data(object data)
            {
                _response.Data = data;
                return this;
            }

            public Response Build()
            {
                return _response;
            }
        }
    }
}