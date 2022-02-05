# DevNotePadNet
Notepad for developers with a clean and simple UI

# Environment
The language is C# and runs with .NET 6 using WPF. No other Nuget packages are in use

# Features
Beside the default features, the following features are planned:
- Parsing and Formatting JSON
- Parsing and Formatting XML
- Encode/Decode Base64
- Reading binary formats

# Releases

## Release 1.3 / 2.x
- Depending on work load and motivation / feature set of WPF. I will not add a 3rd party control i.e.

## Release 1.2.x
- User / Admin Mode indication
- XML Validation tool

## Release 1.2.1
Improvements compared to R1.2

- ScratchPad is optional and only visible if required or enabled
- The commands are only enabled if they are applicable. I.e. Save as Binary is not enabled if the current content is a text.
- The About Box links to the web page

## Release 1.2
This is the first official release of the DevNotePad editor using .NET 6.0 and targets Windows WPF.

## Features (beside the classic editor features) :

- JSON Format / Tree renderer
- XML Format / Tree Renderer
- Text Operations and Hex Byte features
- Base64 Encode/Decode
- UICC features like Applet Converting and TLV decoding
- Open and Save Binary files as Hex Strings
- Scratchpad for storing content for later
- Reload feature.

A few words about the XML formatter. The editor parses the XML as a document. XML snipplets and other may not work. Feel free raising issues with the exception + file. I am trying to find a better implementation, but this will probably result in a release 2.x and the minor release only contains bugfixes.

The editor detects if the opened file has been updated and asks the user for permission before saving. The reload feature reloads the content of an already open file. Think of a log file as an example.

## Known bugs:

The column calculation runs into an issue when dealing with large files.
