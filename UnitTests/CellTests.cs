using Microsoft.VisualStudio.TestTools.UnitTesting;
using hackathon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hackathon.Tests
{
    [TestClass()]
    public class CellTests
    {
        [TestMethod]
        public void EntropySanityCheck()
        {
            Cell cell = new();
        }
    }
}