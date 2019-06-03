namespace RegexGroupProvider.Runtime

open System
open System.Collections.Concurrent
open System.Text.RegularExpressions

type MatchNotSuccessfulException() = 
    inherit Exception()

// Put any utilities here
[<AutoOpen>]
module public Utilities = 
    let s = ConcurrentDictionary<string, Regex>()
    let getRegex regexString = 
        s.GetOrAdd(regexString, fun x -> Regex(regexString, RegexOptions.Compiled))

    let matchStringToRegex regexString stringToMatch = 
        let r = getRegex regexString // Need to recreate this inside the quotation. This quotation is in the run-time context, not in the build context.      
        let m = r.Match(stringToMatch) // This is unsupported.
        if m.Success then m else raise (MatchNotSuccessfulException())

/// Used for checking assembly is referenced correctly via type provider and not direct reference at runtime.
type DummyType = { NotSet: int }

// Put the TypeProviderAssemblyAttribute in the runtime DLL, pointing to the design-time DLL
[<assembly:CompilerServices.TypeProviderAssembly("RegexGroupProvider.DesignTime.dll")>]
do ()
