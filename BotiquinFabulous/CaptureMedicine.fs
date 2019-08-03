module CaptureMedicine

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Plugin.Media
open Botiquin


type Image = byte[] option

// Model
type Model = {
    image : Image
}

// Messages
type Msg = 
    | Nothing
    | TakePhoto
    | PhotoSelected of Image
    | SelectFromGallery
    | NoCamaraAvailable
    | NoImageReturned
    | SelectFromGalleryNotAvailable
    

// Init

let initModel = { image = None }
let init () = { image = None }, Cmd.none


// Functions

let private takePhoto = 
    async {
        let! result = CrossMedia.Current.Initialize() |> Async.AwaitTask
        if not CrossMedia.Current.IsCameraAvailable || not CrossMedia.Current.IsTakePhotoSupported
        then
            return NoCamaraAvailable
        else 
            let cameraOptions = Abstractions.StoreCameraMediaOptions(Directory = "Botiquin", Name="Medicina.jpg")
            let! file = CrossMedia.Current.TakePhotoAsync cameraOptions |> Async.AwaitTask 
            match file with
            | null -> return NoImageReturned
            | _ ->
                let bytes = file.GetStream() |> Utils.streamToArray
                let image = Some bytes
                return PhotoSelected image 
    }

let private selectPhoto = 
    async {
        let! result = CrossMedia.Current.Initialize() |> Async.AwaitTask
        if not CrossMedia.Current.IsPickPhotoSupported
        then
            return SelectFromGalleryNotAvailable
        else 
            let pickPhotoOptions = Abstractions.PickMediaOptions()
            let! file = CrossMedia.Current.PickPhotoAsync pickPhotoOptions |> Async.AwaitTask 
            match file with
            | null -> return NoImageReturned
            | _ ->
                let bytes = file.GetStream() |> Utils.streamToArray
                let image = Some bytes
                return PhotoSelected image 
    }

// Update
let update msg model = 
    match msg with
    | Nothing -> model, Cmd.none
    | TakePhoto -> { model with image = None }, Cmd.ofAsyncMsg(takePhoto)
    | PhotoSelected img -> { model with image = img }, Cmd.none
    | SelectFromGallery -> { model with image = None }, Cmd.ofAsyncMsg(selectPhoto)
    | NoCamaraAvailable -> model, Cmd.ofAsyncMsg(Nothing |> Alerts.displayAlert { title = "Error"; message = "Cámara no disponible en dispositivo"; ok = "OK" })
    | NoImageReturned -> model, Cmd.ofAsyncMsg(Nothing |> Alerts.displayAlert { title = "Error"; message = "No se ha seleccionado ninguna imagen"; ok = "OK" })
    | SelectFromGalleryNotAvailable -> model, Cmd.ofAsyncMsg(Nothing |> Alerts.displayAlert { title = "Error"; message = "Acceso a la galería de imágenes no permitida"; ok = "OK" })


// View
let view (model:Model) dispatch =
    View.ContentPage(
        View.StackLayout(orientation = StackOrientation.Vertical, horizontalOptions = LayoutOptions.Center, verticalOptions = LayoutOptions.Center,
            children = [
                View.Button(text="Cámara", command= (fun () -> dispatch TakePhoto))
                View.Button(text="Seleccionar de la galeria", command = (fun () -> dispatch SelectFromGallery))
            ]
        )
    )