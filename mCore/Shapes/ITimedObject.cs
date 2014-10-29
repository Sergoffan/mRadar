using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace mCore.Shapes
{
    interface ITimedObject
    {
        void UpdateTime(byte weeksWithoutPay, ulong timeleft);
    }
}
