# What is this?

KeyboardRemapper is an utility that allows you to map your keyboard keys to some other keyboard keys you want. It is made to work on Windows and on Windows only (at least right now).\
The way it works is the following: when you press any key anywhere program will notice that and find out whether you've mapped any key to the pressed one. If you did it will redirect key down and key up events to the mapped one.\
Due to the way it works it has to be running all the way through your usual usage (unlike the registry mappers like SharpKeys). To not bother you with launching it every time it has an option to automatically start minimized with windows.

# Why should I use it?

The main reason should be "nothing works, I am depressed" or "wow, so simple". The reason this program was created was the combination of the aforementioned ones. SharpKeys didn't see the keys on my keyboard (media ones) and AutoHotKeys would definitely (I hope) work but "aah, so hard, don't want to write scripts". And the last reason is it looks like 2016-2018 year program in terms of the interface IMHO.

# How do I use it?

## Want to install?

Go to https://github.com/And42/KeyboardRemapper/releases, download the latest version zip, extract it to a folder you want it to live in and you are ready to launch it.\
**Important:** do not install it to the directory that needs admin rights to write in (`Program Files` for example) or give it the admin rights on launch if you can't change your mind. The reason for this is that the settings file is located in the same drectory with the app .exe, and the app needs to modify it to remember what has been already done.

## Installed, what's next?

Launch the program, click on the `Add` button. Choose the source key from the left dropdown menu or click the `Record source key` button and then press the key to want to map (or you can type the key code manually in the left field if it's faster). Then do the same with the key you want to map to (the right side) and click `Apply`. Your key mapping will be shown in the list of the currently mapped keys.\
Add as many keys as you want (you can remove existing keys too; just select one and click `Delete`), check `Run when computer starts`, minimize window (do not close it!) and check your keys (actually you can check them right after you've added them).\
App icon will be shown in the tray where you can click on it to restore the main window.

# Any other cool features?

Not so many actually but just to mention:
* Two available themes: Light and Dark; you can choose the needed one from the dropdown list in the right botton corner
* Export and import mappings to/from a json file (you can change it in any text editor of your choice)

# Restrictions?

Hmm, yeah, have a bit. It is running only with .Net Framework 4.7.2+. You can install it on the Windows 7 and above as a consequence of that. Windows XP is a bit old (you can argue that it is still the best one and is used by a lot of people but just want to have all the new features that exist in .Net Framework above 4.0) and Windows Vista was born to die :( So, I decided to support only Windows 7+.
