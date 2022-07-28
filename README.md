# PokemonCreatorBlazor

A Pokemon file and species editor built in dotnet Blazor, can be used as a base for a file or entity editor. Uses base [PKHeX by kwsch](https://github.com/kwsch/PKHeX) and [PKHeX-Plugins by architdate](https://github.com/architdate/PKHeX-Plugins) as dependencies.

# Limitations

Known issues stem from locals size being too large due to how PKHeX stores encounterdata in large local arrays. 
For example; the constructor for Encounters8Nest.cs will throw a `System.InvalidProgramException`, so encounter searching or showdown set parsing is not stable at this time.

Saving local files in the webasm storage using Blazor causes crashes on everything besides Edge at this time (thanks Microsoft!) so saving the file you've edited requires a bit more work, likely JSInterop.

# Screenshots
![chrome_2022-07-28_16-39-26](https://user-images.githubusercontent.com/66521620/181580501-6801da2e-f32c-4217-81a0-ede8e1aa1f3a.png)
