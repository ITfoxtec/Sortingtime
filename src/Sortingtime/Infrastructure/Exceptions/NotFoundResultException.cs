using System;

namespace Sortingtime
{

    [Serializable]
    public class NotFoundResultException : Exception
    {
        public NotFoundResultException() { }
        public NotFoundResultException(string message) : base(message) { }
        public NotFoundResultException(string message, Exception inner) : base(message, inner) { }
        protected NotFoundResultException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
