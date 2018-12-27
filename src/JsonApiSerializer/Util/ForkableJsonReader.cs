using Newtonsoft.Json;

namespace JsonApiSerializer.Util
{
    internal class ForkableJsonReader : JsonReader
    {
        public readonly object SerializationDataToken;

        protected readonly JsonReader InnerReader;
        
        protected readonly string ParentPath;

        private JsonReaderState _readerState;

        public string FullPath => (ParentPath + "." + Path).Trim('.');

        public ForkableJsonReader(JsonReader reader) 
            : this(reader, new JsonReaderState(reader.TokenType, reader.Value), reader)
        {

        }

        public ForkableJsonReader(JsonReader reader, object serializationDataToken)
          : this(reader, new JsonReaderState(reader.TokenType, reader.Value), serializationDataToken)
        {

        }

        private ForkableJsonReader(JsonReader reader, JsonReaderState state, object serializationDataToken)
        {
            InnerReader = reader;
            ParentPath = reader.Path;
            SetToken(state.Token, state.Value);
            _readerState = state;
            SerializationDataToken = serializationDataToken;
        }



        public override bool Read()
        {
            if(_readerState.Next != null)
            {
                _readerState = _readerState.Next;
                SetToken(_readerState.Token, _readerState.Value);
                return true;
            }
            else
            {
                var result = InnerReader.Read();
                SetToken(InnerReader.TokenType, InnerReader.Value);
                _readerState.Next = new JsonReaderState(TokenType, Value);
                _readerState = _readerState.Next;
                return result;
            }
        }

        public ForkableJsonReader Fork()
        {
            return new ForkableJsonReader(InnerReader, _readerState, SerializationDataToken);
        }

     
        private class JsonReaderState
        {
            public readonly JsonToken Token;
            public readonly object Value;

            public JsonReaderState Next;

            public JsonReaderState(JsonToken token, object value)
            {
                Token = token;
                Value = value;
            }
        }
    }
}
