using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskOne
{
    public class MetaData
    {
        public int parsed_files { get; set; }
        public int parsed_lines { get; set; }
        public int found_errors { get; set; }
        public List<string> invalid_files { get; set; }

        public override string ToString()
        {
            string filesList="";
            foreach (var f in invalid_files)
            {
                filesList += f + ", ";
            }
            return $"parsed_files: {parsed_files}\nparsed_lines: {parsed_lines}\nfound_errors: {found_errors}\ninvalid_files: [{filesList.TrimEnd(new char[] { ',', ' ' })}]";
            
        }

    }
}
