using System;
using System.Text.Json;

namespace Lab8Csharp.networking.protocol
{
    [Serializable]
    public class Request
    {
        public RequestType Type { get; set; }
        public object? Data { get; set; }  // << schimbat în object

        public Request() { }

        public T GetData<T>()
        {
            return JsonSerializer.Deserialize<T>(Data.ToString());
        }

        public class Builder
        {
            private readonly Request _request = new();

            public Builder Type(RequestType type)
            {
                _request.Type = type;
                return this;
            }

            public Builder Data(object data)
            {
                _request.Data = data;
                return this;
            }

            public Request Build()
            {
                return _request;
            }
        }
    }

}