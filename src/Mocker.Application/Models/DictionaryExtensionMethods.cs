using System.Collections.Generic;
using System.Linq;

namespace Mocker.Application.Models
{
    public static class DictionaryExtensionMethods
    {
        public static bool EqualIgnoringOrder(this Dictionary<string, List<string>>? header1, Dictionary<string, List<string>>? header2)
        {
            var header1List = header1.OrderBy(x => x.Key).ToList();
            var header2List = header2.OrderBy(x => x.Key).ToList();

            if (header1List?.Count != header2List?.Count)
            {
                return false;
            }

            for (var i = 0; i < header1.Count(); i++)
            {
                if (header1List?[i].Key != header2List?[i].Key)
                {
                    return false;
                }

                for (var n = 0; n < header1List?[i].Value.Count; n++)
                {
                    if (header1List[i].Value[n] != header2List?[i].Value[n])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
