namespace App

open Feliz

module ComponentsHelper =

    open Fable.Core
    open Fable.Core.JsInterop

    let Logo: string = importDefault "./img/felizlogo.svg"

open Ears.Model
type Components =
    

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member Counter() =
        let (requirementsText, setRequirementsText) = React.useState ("")
        let (requirements, setRequirements) = React.useState ([])
        let (errorMessage, setErrorMessage) = React.useState ("")
        let parseRequirements requirementsText =
            let parseResult = parse requirementsText
            printfn "%A" parseResult
            setRequirements(parseResult |> Result.toList)
            setErrorMessage(match parseResult with | Error err -> err | _ -> "")

        Html.div [
            prop.className "flex min-h-screen bg-gray-100"
            prop.children [
                Html.div [
                    prop.className "container flex flex-col gap-2 [&_h1]:text-4xl items-center mx-auto pt-12"
                    prop.children [
                        Html.img [ prop.src ComponentsHelper.Logo; prop.className "w-48 h-48" ]
                        Html.h1 [ prop.testId "counter-display"; prop.text "EARS parser" ]
                        Html.textarea [
                            prop.rows 4
                            prop.cols 40
                            prop.defaultValue requirementsText
                            prop.onChange (
                                fun (e: Browser.Types.Event) ->
                                    let inputElement = e.target :?> Browser.Types.HTMLInputElement
                                    setRequirementsText(inputElement.value))
                        ]
                        Html.div [prop.className "text-danger"; prop.text errorMessage]
                        Html.button [
                            prop.testId "inc-btn"
                            prop.className
                                "rounded bg-blue-500 text-white px-4 py-2 shadow cursor-pointer hover:bg-blue-600 transition-colors active:scale-95 text-lg"
                            prop.onClick (fun _ -> parseRequirements requirementsText)
                            prop.text "Increment"
                        ]
                    ]
                ]
            ]
        ]
