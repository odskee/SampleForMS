using SampleForMS.Components.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleForMS.Components.Controller
{
    public class ExampleController : IExampleController
    {
        private readonly bool WritingMeWorkedFine = true;
        private string ExampleString = string.Empty;


        public ExampleController()
        {

        }


        public string GetTestString()
        {
            return ExampleString;
        }
        public void SetTestString(string _val)
        {
            ExampleString = _val;
        }
    }
}
