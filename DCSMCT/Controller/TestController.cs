using SampleForMS.Components.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleForMS.Components.Controller
{
    public class TestController : ITestController
    {
        private readonly IExampleController ExampleController;

        public TestController(IExampleController exampleController)
        {
            ExampleController = exampleController;
        }


        public string DoStringSomething()
        {
            var _testString = ExampleController.GetTestString();
            if (!string.IsNullOrEmpty(_testString))
            {
                return $"Test String Value: {_testString.Trim()}";

            }

            return string.Empty;
        }

    }
}
