namespace Domain.Helpers
{
    /// <summary>
    /// application/x-www-form-urlencoded (y algunos clientes) convierten '+' en espacio.
    /// El binder de login es JSON; si el identifier ya llega con espacio en el local-part
    /// (p. ej. qa.smoke+stg@… → "qa.smoke stg@…"), se restaura el '+'.
    /// </summary>
    public static class AccessIdentifierNormalizer
    {
        public static string Normalize(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return identifier;
            }

            var trimmed = identifier.Trim();
            var at = trimmed.IndexOf('@');
            if (at <= 0)
            {
                return trimmed;
            }

            var local = trimmed.Substring(0, at).Replace(' ', '+');
            return local + trimmed.Substring(at);
        }
    }
}
