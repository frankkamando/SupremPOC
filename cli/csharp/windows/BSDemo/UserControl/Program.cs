using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private UserControl uc = new UserControl();

        protected override void runImpl(UInt32 deviceID)
        {
            uc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for user management";
            program.run();
        }
    }

}
