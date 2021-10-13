using System;

namespace DiscordTranslatorBot.Exceptions
{
    public class MissingEnvironmentVariableException : Exception
    {
        public MissingEnvironmentVariableException(string variableName) : base(variableName)
        {
        }
    }
}