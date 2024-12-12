using System.Collections.Generic;
using System.Threading.Tasks;

namespace vBase.Core.Base;

public interface ICommunicationChannel
{
  Task<Dictionary<string, string>> CallMethod(string methodName, params object[] arguments);
}