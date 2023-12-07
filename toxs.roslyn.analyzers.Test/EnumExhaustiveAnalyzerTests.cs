using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using toxs.roslyn.analyzers.Enum;
using toxs.roslyn.analyzers.PopulateSwitch;
using VerifyCS = toxs.roslyn.analyzers.Test.CSharpAnalyzerVerifier<toxs.roslyn.analyzers.PopulateSwitch.PopulateSwitchStatementDiagnosticAnalyzer>;

namespace toxs.roslyn.analyzers.Test
{
    [TestClass]
    public class EnumExhaustiveAnalyzerTests
    {
        [TestMethod]
        public async Task Test_SwitchWithAllCases()
        {
            var test = @"using System;

namespace TestData.Enums.SwitchOnEnumMustHandleAllCases.DiagnosticAnalyzer
{
    public class SwitchWithAllCases
    {
        public void EnumerationMethod(CarModels carModel)
        {
            switch (carModel)
            {
                case CarModels.Ferrari:
                    break;
                case CarModels.Lamborghini:
                    break;
                case CarModels.Mercedes:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null);
            }
        }

        public enum CarModels
        {
            Ferrari,
            Lamborghini,
            Mercedes
        }
    }
}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task Test_SwitchWithAllCasesGrouped()
        {
            var test = @"using System;

namespace TestData.Enums.SwitchOnEnumMustHandleAllCases.DiagnosticAnalyzer
{
    public class SwitchWithAllCases
    {
        public void EnumerationMethod(CarModels carModel)
        {
            switch (carModel)
            {
                case CarModels.Ferrari:
                case CarModels.Lamborghini:
                case CarModels.Mercedes:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null);
            }
        }

        public enum CarModels
        {
            Ferrari,
            Lamborghini,
            Mercedes
        }
    }
}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task Test_SwitchWithAllBoolCases()
        {
            var test = @"using System;

namespace TestData.Enums.SwitchOnEnumMustHandleAllCases.DiagnosticAnalyzer
{
    public class SwitchWithAllCases
    {
        public void EnumerationMethod(bool b)
        {
            switch (b)
            {
                case true:
                    break;
                case false:
                    break;
            }
        }
    }
}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task Test_SwitchWithMissingCase()
        {
            var test = @"using System;

namespace TestData.Enums.SwitchOnEnumMustHandleAllCases.DiagnosticAnalyzer
{
    public class SwitchWithMissingCase
    {
        public void EnumerationMethod(CarModels carModel)
        {
            switch (carModel)
            {
                case CarModels.Ferrari:
                    break;
                case CarModels.Lamborghini:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null);
            }
        }

        public enum CarModels
        {
            Ferrari,
            Lamborghini,
            Mercedes
        }
    }
}
";

            var expectedDiagnostic = new DiagnosticResult(new PopulateSwitchStatementDiagnosticAnalyzer().Rule)
                .WithSpan("/0/Test0.cs", 9, 13, 9, 19)
                .WithSpan(9, 13, 17, 14);
            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task Test_SwitchWithMissingCaseGrouped()
        {
            var test = @"using System;

namespace TestData.Enums.SwitchOnEnumMustHandleAllCases.DiagnosticAnalyzer
{
    public class SwitchWithMissingCase
    {
        public void EnumerationMethod(CarModels carModel)
        {
            switch (carModel)
            {
                case CarModels.Ferrari:
                case CarModels.Lamborghini:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null);
            }
        }

        public enum CarModels
        {
            Ferrari,
            Lamborghini,
            Mercedes
        }
    }
}
";

            var expectedDiagnostic = new DiagnosticResult(new PopulateSwitchStatementDiagnosticAnalyzer().Rule)
                .WithSpan("/0/Test0.cs", 9, 13, 9, 19)
                .WithSpan(9, 13, 16, 14);
            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task Test_SwitchWithoutDefaultCase()
        {
            var test = @"namespace TestData.Enums.SwitchOnEnumMustHandleAllCases.DiagnosticAnalyzer
{
    public class SwitchWithoutDefaultCase
    {
        public void EnumerationMethod(CarModels carModel)
        {
            switch (carModel)
            {
                case CarModels.Ferrari:
                    break;
                case CarModels.Lamborghini:
                    break;
                case CarModels.Mercedes:
                    break;
            }
        }

        public enum CarModels
        {
            Ferrari,
            Lamborghini,
            Mercedes
        }
    }
}
";

            var expectedDiagnostic = new DiagnosticResult(new PopulateSwitchStatementDiagnosticAnalyzer().Rule)
                .WithSpan("/0/Test0.cs", 7, 13, 7, 19)
                .WithSpan(7, 13, 15, 14);
            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}
