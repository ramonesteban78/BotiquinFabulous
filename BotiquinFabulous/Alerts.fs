module Alerts

open Xamarin.Forms

type Alert = 
    {
        title : string
        message : string
        ok : string
    }

let displayAlert alert msg = 
    async {
        let! result = Application.Current.MainPage.DisplayAlert(alert.title, alert.message, alert.ok) |> Async.AwaitTask
        return msg
    }
