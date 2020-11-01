using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xrm.DevOps.Solutions.Libraries
{
    struct FileHeader
    {
        public String FileName;
        public String Extension;
        public String RelativeFilePath;
        public String FullPath;
        public String DirectoryPath;
        public DateTime ModifiedOn;
        internal String Content;
    }
}
