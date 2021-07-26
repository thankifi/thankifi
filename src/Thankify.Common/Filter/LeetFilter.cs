using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Thankify.Common.Filter
{
    public class LeetFilter
    {
        public static string Apply(string original)
        {
            var stringBuilder = new StringBuilder(original.Length);
            
            foreach (var character in original)
            {
                stringBuilder.Append(LeetDict.TryGetValue(character, out var newChar) ? newChar : character);
            }

            return stringBuilder.ToString();
        }
        
        public static ReadOnlyDictionary<char, char> LeetDict => new ReadOnlyDictionary<char, char>(new Dictionary<char, char>
        {
            ['a'] = '4',
            ['b'] = '8',
            ['e'] = '3',
            ['g'] = '9',
            ['i'] = '1',
            ['l'] = '1',
            ['o'] = '0',
            ['q'] = 'k',
            ['s'] = '5',
            ['t'] = '7',
            ['z'] = '2',
            ['A'] = '4',
            ['B'] = '8',
            ['E'] = '3',
            ['G'] = '6',
            ['I'] = '1',
            ['O'] = '0',
            ['Q'] = 'O',
            ['S'] = '5',
            ['T'] = '7',
            ['Z'] = '2',
            ['0'] = 'O',
            ['1'] = 'l',
            ['2'] = 'z',
            ['3'] = 'E',
            ['4'] = 'A',
            ['5'] = 'S',
            ['6'] = 'G',
            ['7'] = 'T',
            ['8'] = 'B',
            ['9'] = 'g',
        });
    }
}