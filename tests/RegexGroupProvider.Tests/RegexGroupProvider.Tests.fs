module RegexGroupProvider.Tests.RegexGroupProviderTests

open FSharp.RegexGroup
open Expecto
open RegexGroupProvider.Runtime
open TestHelpers

type RegexProviderTestOne = RegexGroupProvider<"test(?<group1>.*)">

type RegexProviderTestTwo = RegexGroupProvider<"test(?<group1>.*)/(?<group2>.*)">

let ``valid regex should work and extract value``() = 
    let m = RegexProviderTestOne.Match("test2345")
    Expect.equal m.group1.Value "2345" "Regex not parsed correctly"

let ``invalid string throws exception``() = 
    Expect.throwsT<MatchNotSuccessfulException> (fun () -> RegexProviderTestOne.Match("1234") |> ignore) "Match was a success when it shouldn't of been"

let ``more than one group can be matched``() = 
    let m = RegexProviderTestTwo.Match("test2345/testagain")
    Expect.equal m.group1.Value "2345" "First part of regex not parsed correctly"
    Expect.equal m.group2.Value "testagain" "Second part of regex not parsed correctly"

[<Tests>]
let testList = 
    runTests
        "Type Provider Test"
        [ <@ ``valid regex should work and extract value`` @>
          <@ ``invalid string throws exception`` @>
          <@ ``more than one group can be matched`` @> ]