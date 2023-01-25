<p align="center">
   <img src="https://img.shields.io/badge/Unity%20version-2020.3.21f1-lightgrey" alt="Unity Version">
   <img src="https://img.shields.io/badge/License-MIT-green" alt="License">
</p>

## About
Stub application

The application checks several conditions before launching, it connects to Firebase Remote Config and receives a link from there.

#### Conditions:
- **-** **`Emulator`** - Сhecks if the application is not running from the emulator.
- **-** **`Sim card`** - Сhecks if the device has a sim card.
- **-** **`Link`** - Parameter "link" in the Firebase Remote Config is not null.

If one of this conditions is false, app opens stub (in this case, stub is 30-day weight loss plan), else app opens webview with a link from Firebase Remote Config.
The app also saves the link, if it is and when opening the application, if it finds a saved link, it immediately opens it in webview.
   
This app can be used for to hide betting, gambling or dating from Google play.

## Developers

- [dimokrit](https://github.com/dimokrit)
