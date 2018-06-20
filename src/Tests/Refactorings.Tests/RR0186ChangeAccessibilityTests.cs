﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0186ChangeAccessibilityTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ChangeAccessibility;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeAccessibility)]
        public async Task Test_Method()
        {
            await VerifyRefactoringAsync(@"
class C
{
    [||]public string M() => null;
}
", @"
class C
{
    internal string M() => null;
}
", equivalenceKey: EquivalenceKey.Join(RefactoringId, nameof(Accessibility.Internal)));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeAccessibility)]
        public async Task Test_MultipleDeclarations()
        {
            await VerifyRefactoringAsync(@"
class C
{
[|    public override string ToString() => null;
    internal string M() => null;|]
}
", @"
class C
{
    public override string ToString() => null;
    public string M() => null;
}
", equivalenceKey: EquivalenceKey.Join(RefactoringId, nameof(Accessibility.Public)));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeAccessibility)]
        public async Task TestNoRefactoring_OverriddenDeclarationWithoutBaseSource()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    [||]public override string ToString() => null;
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeAccessibility)]
        public async Task TestNoRefactoring_OverriddenDeclarationsWithoutBaseSource()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
[|    public override string ToString() => null;
    public override int GetHashCode() => 0;|]
}
", equivalenceKey: RefactoringId);
        }
    }
}