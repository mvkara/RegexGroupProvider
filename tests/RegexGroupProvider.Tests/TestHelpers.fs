module RegexGroupProvider.Tests.TestHelpers

open Expecto
open FSharp.Quotations
open Patterns

/// Allows you to use the F# space method name feature to also name the test case.
let public testCaseAsync (methodNameQuotation: Expr<unit -> Async<unit>>) =    
    match methodNameQuotation with
    | Lambda(_, Call(_, methodInfo, _)) -> testCaseAsync methodInfo.Name (methodInfo.Invoke(null, [||]) :?> Async<unit>)
    | _ -> failwith "Not possible due to type constraint."

/// Allows you to use the F# space method name feature to also name the test case.
let public testCase (methodNameQuotation: Expr<unit -> unit>) =    
    match methodNameQuotation with
    | Lambda(_, Call(_, methodInfo, _)) -> testCase methodInfo.Name (fun () -> (methodInfo.Invoke(null, [||]) :?> unit))
    | _ -> failwith "Not possible due to type constraint."
    
let public runTestsAsync testSuiteName testsAsync = testList testSuiteName (testsAsync |> List.map testCaseAsync)

let public runTests testSuiteName tests = testList testSuiteName (tests |> List.map testCase)