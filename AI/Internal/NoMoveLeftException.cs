using System;
using System.Runtime.Serialization;

namespace Stratego.AI
{
   [Serializable]
   internal class NoMoveLeftException : Exception
   {
      public NoMoveLeftException() { }
      public NoMoveLeftException(string message) : base(message) { }
      public NoMoveLeftException(string message, Exception innerException) : base(message, innerException) { }
      protected NoMoveLeftException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}