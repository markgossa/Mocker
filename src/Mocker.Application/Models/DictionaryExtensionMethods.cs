using System.Collections.Generic;

namespace Mocker.Application.Models
{
    public static class DictionaryExtensionMethods
    {
        public static bool Contains(this Dictionary<string, List<string>> requestHeaders, 
            Dictionary<string, List<string>> filterHeaders)
        {
            foreach (var filterHeader in filterHeaders)
            {
                if (!requestHeaders.TryGetValue(filterHeader.Key, out var requestHeaderValue))
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

        public static bool IsEqualTo(this Dictionary<string, string> requestQuery, Dictionary<string, string> filterQuery)
        {
            foreach (var filterQueryItem in filterQuery)
            {
                if (!requestQuery.TryGetValue(filterQueryItem.Key, out var requestQueryValue) || 
                    filterQueryItem.Value != requestQueryValue)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
