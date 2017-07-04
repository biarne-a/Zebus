using System;
using System.Collections.Generic;

namespace Abc.Zebus.Core
{
    public interface IBusSendHistoryContainer
    {
        IDictionary<string, DateTime> GetSentCommands();
        IDictionary<string, DateTime> GetPublishedEvents();
    }
}