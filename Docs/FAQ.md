# 📋 Frequently Asked Questions:

> ## How do I use this?
> 1. Download on the [<u>Latest Release</u>](https://github.com/ChrisFeline/ToNSaveManager/releases/latest), the compressed file called `ToNSaveManager.zip`.
> 2. Extract the contents of the **.zip** file into a folder of your choice.
> 3. Open `ToNSaveManager.exe`.
> 4. Select the log date on the left, then click one of the saves in the right.
> 5. Your code is now in the clipboard, go to VRChat and paste the code in the input field.

> ## Where can I request a feature?
> If you want to suggest new features or changes, you can open an [Issue](https://github.com/ChrisFeline/ToNSaveManager/issues) here or you can join the [official Save Manager Discord Server](https://discord.gg/Anpm8d3fPD) and find me as @**Kittenji**

> ## How does it work?
> The world periodically saves a snapshot of your progress in the VRChat log files.
> 
> Initially, the program will scan your logs at `%LOCALAPPDATA%Low\VRChat\VRChat` and recover previous **Save Codes** in your logs. Then it will continue detecting new codes as you play.

> ## Why is it not showing anything despite previously playing Terrors?
> There's the possibility that you have **logging** disabled.
> You can enable it by opening your Quick Menu, go to settings, scroll all the way down and turn logging **on**.
> <details>
> <summary><b>Show Image</b></summary>
> <p> <img src="Resources/logging.png" height="420px" > </p>
> </details>

> ## Program is stuck on "Loading, please wait..."
> This problem is most likely caused when the program is trying to parse large log files created by VRChat.
> <br>You can check your VRChat logs by pressing `Windows Key` + `R`
> Then type `%APPDATA%\..\LocalLow\VRChat\VRChat` and click **Ok**.
> <br>If you see logs file with a storage size out of the ordinary, that might be the cause of your issue.

> ## How to properly configure Automatic Discord Backup using Webhooks?
> You can set a Discord webhook url to automatically upload your codes to a discord channel.
>
> - Just go to your preferred channel on your discord server.
> - Click **Edit Channel** and then go to **Integrations**.
> - Add a webhook integration to this channel. *You can give it a name and a profile picture*.
> - Copy the webhook url.
> - Open settings on the Save Manager app.
> - Enable `Auto Discord Backup`, you will see a text input popup.
> - Paste your webhook url in the text field.
> - Click **save**.
> 
> NOTE: If you want to test this functionality, you can right click on a save entry then click **Backup**. If everything is right, save will be uploaded to the discord channel.
> <details>
> <summary><b>Show Discord Screenshots</b></summary>
> <p> <img src="Resources/Webhook/screenshot_0.png" height="auto"> </p>
> <p> <img src="Resources/Webhook/screenshot_1.png" width="682px"> </p>
> <p> <img src="Resources/Webhook/screenshot_2.png" width="512px"> </p>
> </details>

> ## Why is the .exe so big? >100MB
> The exe is bundled with the .NET runtime that it's required to run the program. Using a command line argument for dotnet publishing: `--self-contained true -p:PublishSingleFile=true` <br>
> This adds size to the file, but ensures that the program runs independently without relying on a previous .NET installation.
>
> This is so people that download this program does not have to go download the .NET runtime framework themselves. And it's ready to run without any extra actions from the user.
>
> The program is compiled from the source using Github actions, you can see the full arguments in [the workflow file](https://github.com/ChrisFeline/ToNSaveManager/blob/a0d503b02fe25fde1b36ca9807756f1830c8e7a8/.github/workflows/dotnet-desktop.yml#L46C45-L46C45).


> ## Is this against VRChats ToS?
> - **Short Answer:** No
>
> This is an external tool that uses local plain text files that VRChat writes to the Local APPDATA folder.
> We are allowed to read these files since it does not modify or alter the game in any way.
> **This is not a mod or a cheat.**