using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageOffice.classes.LogServices;
using StorageOffice.classes.Tests.Interfaces;

namespace StorageOffice.classes.Tests.Abstractions
{
    internal class TestsBase : ITests
    {
        protected Action? _tests;

        public virtual bool RunAllTests()
        {
            if (_tests != null)
            {
                LogManager.AddNewLog("Info: system service tests started");
                var originalOutput = Console.Out;
                Console.SetOut(TextWriter.Null);
                string results = "";
                bool everyTestPassed = true;
                foreach (var test in _tests.GetInvocationList())
                {
                    try
                    {
                        test.DynamicInvoke();
                    }
                    catch (Exception ex)
                    {
                        everyTestPassed = false;
                        results += "- " + ex.InnerException!.Message + "\n";
                        LogManager.AddNewLog($"Error: {ex.InnerException!.Message}");
                    }
                }
                LogManager.AddNewLog("Info: system service tests ended");
                Console.SetOut(originalOutput);
                Console.WriteLine(results);
                return everyTestPassed;
            }
            return false;
        }
    }
}
