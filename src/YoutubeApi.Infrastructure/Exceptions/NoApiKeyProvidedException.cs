using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace YoutubeApi.Infrastructure.Exceptions
{
    [Serializable]
    internal class NoApiKeyProvidedException : Exception
    {
        public NoApiKeyProvidedException()
        {
        }

        public NoApiKeyProvidedException(string? message) : base(message)
        {
        }

        public NoApiKeyProvidedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoApiKeyProvidedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}