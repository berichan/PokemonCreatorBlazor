# PokemonCreatorBlazor

A Pokemon file and species editor built in dotnet Blazor, can be used as a base for a file or entity editor. Uses base [PKHeX by kwsch](https://github.com/kwsch/PKHeX) and [PKHeX-Plugins by architdate](https://github.com/architdate/PKHeX-Plugins) as dependencies.

# Limitations

Known issues stem from locals size being too large due to how PKHeX stores encounterdata in large local arrays. 
For example; the constructor for Encounters8Nest.cs will throw a `System.InvalidProgramException`, so encounter searching or showdown set parsing is not stable at this time. Note that this exception will be thrown on startup of the webapp, but does not affect the initial functionality.

Saving local files in the webasm storage using Blazor causes crashes on everything besides Edge at this time (thanks Microsoft!) so saving the file you've edited requires a bit more work, likely JSInterop.

# Deployment

This application does not currently have a live demo. You may compile the webapp yourself for local operation or read the Blazor webassembly deployment instructions on the [Microsoft docs for live deployment](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly?view=aspnetcore-3.1#standalone-deployment).

# Screenshots
![chrome_2022-07-28_16-39-26](https://user-images.githubusercontent.com/66521620/181580501-6801da2e-f32c-4217-81a0-ede8e1aa1f3a.png)
