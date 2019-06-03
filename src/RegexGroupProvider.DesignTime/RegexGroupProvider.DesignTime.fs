module RegexGroupProviderImplementation

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open FSharp.Quotations
open FSharp.Core.CompilerServices
open RegexGroupProvider.Runtime
open ProviderImplementation
open ProviderImplementation.ProvidedTypes
open System.Text.RegularExpressions

// Put any utility helpers here
[<AutoOpen>]
module internal Helpers =
    ()

[<TypeProvider>]
type RegexGroupProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, assemblyReplacementMap=[("RegexGroupProvider.DesignTime", "RegexGroupProvider.Runtime")], addDefaultProbingLocation=true)

    let ns = "FSharp.RegexGroup"
    let asm = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    do assert (typeof<DummyType>.Assembly.GetName().Name = asm.GetName().Name)  
    
    let createInnerTypesFromStaticParams (name: string) (regexString: string) = 
        let r = getRegex regexString
        let t = ProvidedTypeDefinition(asm, ns, name, Some typeof<obj>)
        
        let returnType = ProvidedTypeDefinition(asm, ns, "ProvidedType", Some typeof<obj>)
        for groupName in r.GetGroupNames() do
            let pp = ProvidedProperty(groupName, typeof<Group>, getterCode = fun args -> <@@ ((%%(args.[0]) :> obj) :?> Match).Groups.[groupName] @@>)
            returnType.AddMember(pp)

        let matchMethod = 
            ProvidedMethod(
                "Match", 
                [ ProvidedParameter("matchstring", typeof<string>) ], 
                returnType, 
                isStatic = true, 
                invokeCode = fun args -> <@@ matchStringToRegex regexString (%%args.[0]) :> obj @@>)
        
        t.AddMember(matchMethod)
        t.AddMember(returnType)
        t // Make sure all types created and used are added to the assembly.// The types returned by applying the static parameters for the type constructor.

    let createTypes () =
        let myType = ProvidedTypeDefinition(asm, ns, "RegexGroupProvider", Some typeof<obj>)
        myType.DefineStaticParameters(
            [ ProvidedStaticParameter("RegexString", typeof<string>, None) ],
            fun typeName args -> createInnerTypesFromStaticParams typeName (args.[0] :?> string))
        [ myType ]

    do
        this.AddNamespace(ns, createTypes())

[<TypeProviderAssembly>]
do ()
