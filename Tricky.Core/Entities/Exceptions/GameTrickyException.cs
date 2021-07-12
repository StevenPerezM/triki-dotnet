using System;
using System.Runtime.Serialization;

namespace Tricky.Core.Entities.Exceptions
{
    [Serializable]
    public class GameTrickyException : Exception
    {
        public GameTrickyException(string message) : base(message)
        {
        }
        protected GameTrickyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
