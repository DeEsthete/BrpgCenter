using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    //трактуется как - я хочу ....
    public enum FileMessageType
    {
        SendFileList,
        SendThisFile,
        AcceptFileList,
        AcceptThisFile,
        UploadFile,
    }
}
