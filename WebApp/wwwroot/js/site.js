//skript som hanterar uppladdning av en profilbild



// Detta körs när hela webbsidan har laddats.
//En händelse registreras som körs när hela webbsidan har laddats(DOMContentLoaded).
//När detta event inträffar, kommer funktionen däri köras.
document.addEventListener('DOMContentLoaded', function () {
    //anropar funktion
    handleProfileImageUpload()
})

function handleProfileImageUpload() {
    try {
        // letar efter ett element i _ProfileInfo med id 'fileUploader'.
        let fileUploader = document.querySelector('#fileUploader')
        //kontroll om elementet hittas
        if (fileUploader != undefined) {

            //om hittas, lägger vi till en eventListener på elementet,
            //vilken lyssnar efter en förändring i elementet.
            //i Detta fall  när användaren väljer att laddda upp en fil
            fileUploader.addEventListener('change', function () {
                // Denna kod körs när användaren väljer en fil att ladda upp.
                //kontroll om anv valt en fil
                if (this.files.length > 0) {
                    //om true, skickas formuläret med uppladdningsknappen.
                    this.form.submit()
                }
            })
        }
    }
    catch { }
}