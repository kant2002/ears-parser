module Tests

open System
open Xunit
open Ears.Model

let compareParseResult expected result =
    if result <> Ok expected then
        Assert.Fail(sprintf "The results different. Expected %A, but found %A" expected result)

[<Fact>]
let ``Parse ubiquitous requirement`` () =
    let result = parse "The control system shall prevent engine overspeed."
    compareParseResult { 
        Tag = None
        Feature = None
        Precondition = None
        ExpectingTrigger = None
        UnexpectingTrigger = None
        Component = "control system"
        Response = "prevent engine overspeed" } result

[<Fact>]
let ``Parse event driven requirement`` () =
    let result = parse "When continuous ignition is commanded by the aircraft, the control system shall switch on continuous ignition."
    compareParseResult { 
        Tag = None; 
        Feature = None
        Precondition = None
        ExpectingTrigger = Some "continuous ignition is commanded by the aircraft"
        UnexpectingTrigger = None
        Component = "control system"
        Response = "switch on continuous ignition" } result

[<Fact>]
let ``Parse state based requirement`` () =
    let result = parse "During thrust reverser door translation, the control system shall limit thrust to minimum idle."
    compareParseResult { 
        Tag = None
        Feature = None
        Precondition = Some "thrust reverser door translation"
        ExpectingTrigger = None
        UnexpectingTrigger = None
        Component = "control system"
        Response = "limit thrust to minimum idle" } result

[<Fact>]
let ``Parse optional feature requirement`` () =
    let result = parse "Where the control system includes an overspeed protection function, the control system shall test the availability of the overspeed protection function prior to aircraft dispatch."
    compareParseResult { 
        Tag = None
        Feature = Some "control system includes an overspeed protection function"
        Precondition = None
        ExpectingTrigger = None
        UnexpectingTrigger = None
        Component = "control system"
        Response = "test the availability of the overspeed protection function prior to aircraft dispatch" } result
        
[<Fact>]
let ``Parse unwanted behaviour requirement`` () =
    let result = parse "If the computed airspeed fault flag is set, then the control system shall use modelled airspeed."
    compareParseResult { 
        Tag = None; 
        Feature = None
        Precondition = None
        ExpectingTrigger = None
        UnexpectingTrigger = Some "the computed airspeed fault flag is set"
        Component = "control system"
        Response = "use modelled airspeed" } result

[<Fact>]
let ``Parse complex requirement`` () =
    let result = parse "While the aircraft is on-ground, when reverse thrust is commanded, the control system shall enable deployment of the thrust reverser."
    compareParseResult { 
        Tag = None; 
        Feature = None
        Precondition = Some "the aircraft is on-ground"
        ExpectingTrigger = Some "reverse thrust is commanded"
        UnexpectingTrigger = None
        Component = "control system"
        Response = "enable deployment of the thrust reverser" } result

[<Fact>]
let ``Parse complex requirement 2`` () =
    let result = parse "While the aircraft is in-flight, if reverse thrust is commanded, then the control system shall inhibit thrust reverser deployment."
    compareParseResult { 
        Tag = None; 
        Feature = None
        Precondition = Some "the aircraft is in-flight"
        ExpectingTrigger = None
        UnexpectingTrigger = Some "reverse thrust is commanded"
        Component = "control system"
        Response = "inhibit thrust reverser deployment" } result

[<Fact>]
let ``Parse complex requirement 3`` () =
    let result = parse "When selecting idle setting, if aircraft data is unavailable, then the control system shall select Approach Idle."
    compareParseResult { 
        Tag = None; 
        Feature = None
        Precondition = None
        ExpectingTrigger = Some "selecting idle setting"
        UnexpectingTrigger = Some "aircraft data is unavailable"
        Component = "control system"
        Response = "select Approach Idle" } result

[<Fact>]
let ``Parse tagged requirement`` () =
    let result = parse "Prompt_PIN: The software shall prompt the user for the PIN."
    compareParseResult { 
        Tag = Some "Prompt_PIN"
        Feature = None
        Precondition = None
        ExpectingTrigger = None
        UnexpectingTrigger = None
        Component = "software"
        Response = "prompt the user for the PIN" } result