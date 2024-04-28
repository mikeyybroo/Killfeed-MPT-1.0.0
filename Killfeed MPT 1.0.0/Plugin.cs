using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace Killfeed_MPT
{
    [BepInPlugin("mikeyy.mpt.killfeed", "MPT-Killfeed", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
        }
        public void Start()
        {
            new Killfeed().Enable();
        }
    }
}


