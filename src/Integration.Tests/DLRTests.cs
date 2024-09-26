using CSnakes.Runtime.Python;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
namespace Integration.Tests;

public class DLRTests : IntegrationTestBase
{
    [Fact]
    public void TestDLR_TestInvocation()
    {
        var testModule = Env.TestDlr();
        dynamic testObject = testModule.TestInvocation();
        Assert.NotNull(testObject);
        Assert.Equal(typeof(PyObject), ((Object)testObject).GetType());
        dynamic result = testObject.Hello("World");
        Assert.NotNull(result);
        Assert.Equal(typeof(PyObject), ((Object)result).GetType());
        PyObject pyResult = (PyObject)result;
        Assert.Equal("Hello, World", pyResult.ToString());
    }
}
