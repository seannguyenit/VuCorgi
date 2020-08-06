using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Resource.WebCenter
{
    public enum EnumTypeInfo
    {
        ContactImg,
        About,
        Contact,
        QuickQuestion,
        HumanResource
    }

    public enum TaskStatus
    {
        AllStatus,
        New,
        Working,
        Pending,
        Finished,
        Cancel
    }

    public enum FileType
    {
        banner,
        Blog,
        Idea,
        Image
    }

    public enum MessageType
    {
        Contact,
        Request
    }
    public enum NewsType
    {
        News,
        Product
    }

}
