module Ears.Model
open FParsec
open FParsec.CharParsers

type Requirement =
    { 
        Tag: string option
        Feature: string option
        Precondition: string option
        ExpectingTrigger: string option
        UnexpectingTrigger: string option
        Component: string
        Response: string
    }

type Token =
    | TheToken
    | WhileToken
    | WhenToken
    | WhereToken
    | IfToken
    | ThenToken
    | ShallToken
    | DotToken
    | CommaToken
    | ColonToken
    | Word of string

let collectTokens (tokens: Token list) =
    tokens |> List.map (fun t -> match t with | Word t -> t | _ -> failwithf "Only words expected, but %A found." t)
    |> String.concat " "

let wordContinueLetter c =
    isAsciiLetter c || c = '-'

let DotTokenParser = pchar '.' >>% DotToken
let CommaTokenParser = pchar ',' >>% CommaToken
let ColonTokenParser = pchar ':' >>% ColonToken
let TheTokenParser = pstringCI "The" >>% TheToken .>> spaces1
let IfTokenParser = pstringCI "If" >>% IfToken .>> spaces1
let ThenTokenParser = pstringCI "Then" >>% ThenToken .>> spaces1
let WhenTokenParser = pstringCI "When" >>% WhenToken .>> spaces1
let WhileTokenParser = (pstringCI "While" <|> pstringCI "During") >>% WhileToken .>> spaces1
let WhereTokenParser = pstringCI "Where" >>% WhereToken .>> spaces1
let ShallTokenParser = pstringCI "Shall" >>% ShallToken .>> spaces1
let WordParser = (pfloat |>> string |>> Word) <|> (identifier (IdentifierOptions(isAsciiIdContinue = wordContinueLetter)) |>> Word) .>> spaces
let TagParser = identifier (IdentifierOptions())

let StatementParser = 
    TheTokenParser >>. 
    (many1Till WordParser ShallTokenParser |>> collectTokens) 
    .>>. (many1Till WordParser DotTokenParser |>> collectTokens)

let RequirementParser: Parser<Requirement, unit> =
    opt (attempt (TagParser .>> ColonTokenParser .>> spaces))
    .>>. opt (WhereTokenParser >>. spaces >>. TheTokenParser >>. (many1Till WordParser CommaTokenParser |>> collectTokens) .>> spaces)
    .>>. opt (WhileTokenParser >>. (many1Till WordParser CommaTokenParser |>> collectTokens) .>> spaces)
    .>>. opt (WhenTokenParser >>. (many1Till WordParser CommaTokenParser |>> collectTokens) .>> spaces) 
    .>>. opt (IfTokenParser >>. (many1Till WordParser CommaTokenParser |>> collectTokens) .>> spaces .>> ThenTokenParser .>> spaces) 
    .>>. StatementParser |>> 
        (fun (((((tag, feature), state), trigger), unwantedTrigger), (control, statement)) -> 
        {
            Tag = tag
            Feature = feature
            Component = control
            Precondition = state
            ExpectingTrigger = trigger
            UnexpectingTrigger = unwantedTrigger
            Response = statement
        })

let parse text: FSharp.Core.Result<Requirement, string> =
    let parseResult = run RequirementParser text
    match parseResult with
    | Success (result, _, pos) -> FSharp.Core.Ok result
    | Failure (message, error, pos) -> 
        FSharp.Core.Error (sprintf "Error at position %A: %s,  %A" pos message error)