![Idikwa](https://user-images.githubusercontent.com/30344403/149578992-7e9cfae6-c4c3-4570-8c09-1b20a4e503db.png)

# What is Idikwa ?

Idikwa is a recording software aimed to capture the most recent audio played on any peripheral. You can then save to sample to a `.mp3` file.

**Windows only**, audio lib not available on Linux 😔.

## Languages support
* English ![image](https://user-images.githubusercontent.com/30344403/149586346-ccfb58fa-32ff-463e-8c03-3e382257e5a7.png)
* French ![image](https://user-images.githubusercontent.com/30344403/149586447-bba9f999-3f05-4427-bb17-51881503ce12.png)

# How to install

Once you downloaded the zip file [here](https://github.com/Wiltoag/IDIKWA-App/releases/latest), you can just extract it somewhere (`C:\Program Files` for example) and run `Idikwa.exe`.
* **If you downloaded the base version (non portable), you need to install [.NET 6.0 x64 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).**
* **If you downloaded the portable version, be sure to extract it in a folder *without* admins rights.**

# How to use

To start recording, simply click on the big button on the right of the UI :

![button](https://user-images.githubusercontent.com/30344403/149584848-4169c525-bf62-4b40-925e-044575b0ac81.png)

When you click again on it, hit the ![image](https://user-images.githubusercontent.com/30344403/149584978-6d74f701-4b48-414d-9564-c255a953b98d.png) button on the popup. The record will be saved in a specific folder, under the `Idikwa` folder in your `Documents` as a `.mp3` file with the current date as name.

# API support

Idikwa has an API support since v1.1.0. To use it, download the idikwa.api.zip (or the portable version) in the Releases tab. To use it, you need to have a session of Idikwa running. Then you can simply run the idikwa-api.exe in CLI and it will show the help pannel. If there is any error, the response will **always** start with `error:` and a message explaining the error

Side note : You don't need to put idikwa-api.exe (and its dependencies) in the same directory as the main Idikwa.exe, you are free to ship and use the api .exe in any other project.

# Screenshots

![image](https://user-images.githubusercontent.com/30344403/149671218-d1356e77-ccfb-4c27-a423-7ddcd2473c86.png)
![image](https://user-images.githubusercontent.com/30344403/149671244-ffc50931-1ec0-4324-a5f9-6489c1118182.png)
![image](https://user-images.githubusercontent.com/30344403/149585328-64dfee1b-ba47-467f-9f18-6abc025d31ea.png)
![image](https://user-images.githubusercontent.com/30344403/149585599-e8a84304-04fd-470e-8534-9ff4e864c6e7.png)
![image](https://user-images.githubusercontent.com/30344403/149586044-05e79693-71a2-4402-8280-6afdb46c4e43.png)
