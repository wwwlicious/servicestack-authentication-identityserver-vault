namespace ServiceStack.Vault.Core.Helpers
{
    using System;

    public static class PreConditions
    {
        /// <summary>Throws an Argument Null Exception if a string is null or whitespace</summary>
        /// <param name="param">Param Value</param>
        /// <param name="name">Name of the Param</param>
        public static string ThrowIfNullOrWhitespace(this string param, string name)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentNullException(nameof(name));
            }
            return param;
        }
    }
}
