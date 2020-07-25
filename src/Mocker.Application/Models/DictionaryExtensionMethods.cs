using System.Collections.Generic;

namespace Mocker.Application.Models
{
    public static class DictionaryExtensionMethods
    {
        public static bool Contains(this Dictionary<string, List<string>> requestHeaders, Dictionary<string, List<string>> filterHeaders)
        {
            foreach (var filterHeader in filterHeaders)
            {
                var requestHeaderContainsKey = requestHeaders.TryGetValue(filterHeader.Key, out var requestHeaderValue);

                if (!requestHeaderContainsKey)
                {
                    return false;
                }

                for (var i = 0; i < filterHeader.Value.Count; i++)
                {
                    if (requestHeaderValue is null || requestHeaderValue[i] != filterHeader.Value[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
