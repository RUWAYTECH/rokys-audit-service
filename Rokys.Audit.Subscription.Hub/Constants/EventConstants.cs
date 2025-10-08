using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rokys.Audit.Subscription.Hub.Constants
{
    public class EventConstants
    {
        public const string ApplicationCode = "AUDITORIA";
        public struct UserEvents
        {
            public const string UserCreated = "security.user.events.created";
            public const string UserUpdated = "security.user.events.updated";
            public const string UserDeleted = "security.user.events.deleted";
        }
        public struct EmployeeEvents
        {
            public const string EmployeeCreated = "memos.employee.events.created";
            public const string EmployeeUpdated = "memos.employee.events.updated";
            public const string EmployeeDeleted = "memos.employee.events.deleted";
        }
    }
}