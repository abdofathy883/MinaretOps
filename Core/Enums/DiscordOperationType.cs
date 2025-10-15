using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum DiscordOperationType
    {
        NewTask = 0,
        UpdateTask = 1,
        DeleteTask = 2,
        CompleteTask = 3,
        ChangeTaskStatus = 4
    }
}
