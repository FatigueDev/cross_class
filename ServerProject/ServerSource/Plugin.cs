using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barotrauma;

namespace CrossClass
{
    
    public partial class CrossClass : IAssemblyPlugin
    {
        public static bool IsDedicatedServer => GameMain.Server.OwnerConnection == null;

    }
}
