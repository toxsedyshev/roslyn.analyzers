using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using toxs.roslyn.analyzers.Enum;
using toxs.roslyn.analyzers.PopulateSwitch;
using VerifyCS = toxs.roslyn.analyzers.Test.CSharpAnalyzerVerifier<toxs.roslyn.analyzers.PopulateSwitch.PopulateSwitchExpressionDiagnosticAnalyzer>;

namespace toxs.roslyn.analyzers.Test
{
    [TestClass]
    public class EnumExpressionExhaustiveAnalyzerTests
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
            _ = carModel switch
            {
                CarModels.Ferrari => 1,
                CarModels.Lamborghini => 2,
                CarModels.Mercedes => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null),
            };
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
            _ = carModel switch
            {
                CarModels.Ferrari or CarModels.Lamborghini 
                or CarModels.Mercedes => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null),
            };
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
            _ = b switch
            {
                false => 0,
                true => 1,
            };
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
            _ = carModel switch
            {
                CarModels.Ferrari => 1,
                CarModels.Lamborghini => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null),
            };
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

            var expectedDiagnostic = new DiagnosticResult(new PopulateSwitchExpressionDiagnosticAnalyzer().Rule)
                .WithSpan("/0/Test0.cs", 9, 17, 14, 14)
                .WithSpan(9, 17, 14, 14);
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
            _ = carModel switch
            {
                CarModels.Ferrari or CarModels.Lamborghini => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null),
            };
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

            var expectedDiagnostic = new DiagnosticResult(new PopulateSwitchExpressionDiagnosticAnalyzer().Rule)
                .WithSpan("/0/Test0.cs", 9, 17, 13, 14)
                .WithSpan(9, 17, 13, 14);
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
            _ = carModel switch
            {
                CarModels.Ferrari => 1,
                CarModels.Lamborghini => 2,
                CarModels.Mercedes => 3,
            };
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

            var expectedDiagnostic = new DiagnosticResult(new PopulateSwitchExpressionDiagnosticAnalyzer().Rule)
                .WithSpan("/0/Test0.cs", 7, 17, 12, 14)
                .WithSpan(7, 17, 12, 14);
            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }
    }
}