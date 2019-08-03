namespace Botiquin

open System.IO

module Utils =

    let nullToOption value =
        match value with
        | null -> None
        | _ -> Some value

    let streamToArray (stream:Stream) =
        let ms = new MemoryStream()
        stream.CopyTo(ms)
        ms.ToArray()
